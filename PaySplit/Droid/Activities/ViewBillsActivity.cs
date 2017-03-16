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
    [Activity(Label = "PaySplit", MainLauncher = false, Icon = "@mipmap/new_icon")]
    public class ViewBillsActivity : Activity
    {
		private List<Bill> mBills = new List<Bill>();
		private BillListViewAdapter mAdapter;
		private ListView mViewBillsListview;

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

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mNoResultsImage = FindViewById<ImageView>(Resource.Id.NoResultsImage);
            mViewBillsListview = FindViewById<ListView>(Resource.Id.View_ListView);

			LayoutInflater layoutInflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
			View header = (View)layoutInflater.Inflate(Resource.Layout.DateFilterView, null);
			mDateTextView = header.FindViewById<TextView>(Resource.Id.DateFilter_Date_TextView);
			mDateTextView.Text = mFilterTime.ToString("MMMMMMMMM yyyy").ToUpper();
			header.Click += Date_Click;
			mViewBillsListview.AddHeaderView(header);

			// Setup adapter
            mAdapter = new BillListViewAdapter(this, mBills);
            mViewBillsListview.Adapter = mAdapter;

			//Initialize database service
			mDBS = DataHelper.getInstance().getGenDataService();
        }

		protected override void OnResume()
		{
			base.OnResume();
			mBills = mDBS.GetAllBills();
			UpdateListView();
		}
			
		private void UpdateListView()
		{
			string category = Intent.GetStringExtra(Constants.CATEGORY_EXTRA);
			if (category != null && !category.Equals(""))
			{
				mBills = fetchBillsByCategory(mBills, category);
			}
			mBills = fetchBillsForDate(mBills, mFilterTime);

			if (mBills == null || mBills.Count == 0)
			{
				mNoResultsText.Visibility = ViewStates.Visible;
				mNoResultsImage.Visibility = ViewStates.Visible;
			}

			mAdapter.update(mBills);
			mViewBillsListview.Adapter = mAdapter;
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.main_menu, menu);
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
				case Resource.Id.delete_all_bills:
					mDBS.deleteAllBills();
					Toast.MakeText(this, "All Bills Deleted!", ToastLength.Short).Show();
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

		private List<Bill> fetchBillsByCategory(List<Bill> bills, String category)
		{
			if (category == null || category.Equals(""))
			{
				return mBills;
			}

			List<Bill> filteredBills = new List<Bill>();
			foreach (Bill bill in bills)
			{
				if (category.Equals(bill.Category))
				{
					filteredBills.Add(bill);
				}
			}

			return filteredBills;
        }

		private List<Bill> fetchBillsForDate(List<Bill> bills, DateTime date)
		{
			List<Bill> filteredBills = new List<Bill>();
			foreach (Bill bill in bills)
			{
				if ((date.Month) == (bill.Date.Month) && (date.Year) == (bill.Date.Year))
				{
					filteredBills.Add(bill);
				}
			}

			return filteredBills;
		}
    }
}