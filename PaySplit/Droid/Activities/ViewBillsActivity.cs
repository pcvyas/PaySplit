using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PaySplit.Droid
{
    [Activity(Label = "PaySplit", MainLauncher = false, Icon = "@mipmap/ic_launcher")]
    public class ViewBillsActivity : Activity
    {
        private List<Bill> mBills;
		private BillListViewAdapter mAdapter;
		private ListView mViewBillsListview;
        private SearchView searchView;
        private SearchView mSearchView;
        private static string searchString;

		private ImageView mNoResultsImage;
		private TextView mNoResultsText;
		private TextView mDateTextView;

		private DateTime mFilterTime;

		private GenDataService mDBS;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main_ListView);

            mFilterTime = DateTime.Now;
            searchString = "";


			mFilterTime = DateTime.Now;

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mNoResultsImage = FindViewById<ImageView>(Resource.Id.NoResultsImage);
            mViewBillsListview = FindViewById<ListView>(Resource.Id.View_ListView);

            LayoutInflater layoutInflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
			View header = (View)layoutInflater.Inflate(Resource.Layout.DateFilterView, null);
			mDateTextView = header.FindViewById<TextView>(Resource.Id.DateFilter_Date_TextView);
			mDateTextView.Text = mFilterTime.ToString("MMMMMMMMM yyyy").ToUpper();
			header.Click += Date_Click;
			mViewBillsListview.AddHeaderView(header);

            //Initialize database service
            mDBS = DataHelper.getInstance().getGenDataService();

            // Load all bills from database
			mBills = mDBS.GetAllBills().OrderBy(o => o.Date).ToList();

            // Setup adapter
            mAdapter = new BillListViewAdapter(this, mBills);
            mViewBillsListview.Adapter = mAdapter;
        }

        protected override void OnResume()
		{
			base.OnResume();
			UpdateListView();
		}
			
		private void UpdateListView()
		{
			mBills = mDBS.GetAllBills();

			string category = Intent.GetStringExtra(Constants.CATEGORY_EXTRA);

            // we filter the bills using filter decorators
            mBills = new Decorators.SearchFilter(new Decorators.DateFilter(new Decorators.CategoryFilter(mDBS.GetAllBills(), category), mFilterTime), searchString);

			if (mBills == null || mBills.Count == 0)
			{
				mNoResultsText.Visibility = ViewStates.Visible;
				mNoResultsImage.Visibility = ViewStates.Visible;
			} else
            {
                mNoResultsText.Visibility = ViewStates.Gone;
                mNoResultsImage.Visibility = ViewStates.Gone;
            }

			mAdapter.update(mBills);
            mViewBillsListview.Adapter = mAdapter;
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.main_menu, menu);

            searchView = (SearchView)menu.FindItem(Resource.Id.search).ActionView;
            mSearchView = searchView.JavaCast<SearchView>();
            mSearchView.QueryTextChange += (s, e) =>
            {
                searchString = e.NewText;
                UpdateListView();
            };
            mSearchView.QueryTextSubmit += (s, e) =>
            {
                e.Handled = true;
            };
            
            return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.categories:
					StartActivity(typeof(CategoryActivity));
					return true;
				case Resource.Id.add_bill:
					StartActivity(typeof(CreateBillActivity));
					return true;
				case Resource.Id.settings:
					StartActivity(typeof(SettingsActivity));
					return true;
				default:
					return base.OnOptionsItemSelected(item);
			}
		}

        void Date_Click(object sender, EventArgs e)
		{
			DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
																 {
																	 mFilterTime = time;
																	 UpdateListView();
																	 mDateTextView.Text = time.ToString("MMMMMMMMM yyyy").ToUpper();
																 });
			frag.Show(FragmentManager, DatePickerFragment.TAG);
		}
    }
}