using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using Java.Lang;
namespace PaySplit.Droid
{

	[Activity(Label = "Categories", MainLauncher = false, Icon = "@mipmap/new_icon")]
	public class CategoryActivity : Activity
	{
		private List<string> mCategories = new List<string>();
		private CategoryListViewAdapter mAdapter;

		private ListView mCategoriesListview;
		private ImageView mNoResultsImage;
		private TextView mNoResultsText;

		private const string mChartURL = "file:///android_asset/pie_chart.html";
		private GenDataService mDBS;
		private WebView mChartsView;
		private WebAppInterface mWebInterface;

		private TextView mDateTextView;
		private DateTime mFilterTime;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main_ListView);

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mNoResultsImage = FindViewById<ImageView>(Resource.Id.NoResultsImage);
			mCategoriesListview = FindViewById<ListView>(Resource.Id.View_ListView);
			LayoutInflater layoutInflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);


			//Date Filter Header
			mFilterTime = DateTime.Now;
			View headerDate = (View)layoutInflater.Inflate(Resource.Layout.DateFilterView, null);
			mDateTextView = headerDate.FindViewById<TextView>(Resource.Id.DateFilter_Date_TextView);
			mDateTextView.Text = mFilterTime.ToString("MMMMMMMMM yyyy").ToUpper();
			headerDate.Click += Date_Click;
			mCategoriesListview.AddHeaderView(headerDate);



			View header = (View)layoutInflater.Inflate(Resource.Layout.DashboardLayout, null);
			mCategoriesListview.AddHeaderView(header);

			// Setup adapter
			mAdapter = new CategoryListViewAdapter(this, mCategories);
			mCategoriesListview.Adapter = mAdapter;

			//Expense Chart Config
			mChartsView = FindViewById<WebView>(Resource.Id.chartView);
			mChartsView.Settings.JavaScriptEnabled = true;
			mDBS = DataHelper.getInstance().getGenDataService();


			mWebInterface = new WebAppInterface(Resources.GetStringArray(Resource.Array.categories_array));
			mWebInterface.UpdateBills(new Decorators.DateFilter(mDBS.GetAllBills(), mFilterTime));
			mChartsView.AddJavascriptInterface(mWebInterface, "Android");
			mChartsView.LoadUrl(mChartURL);

		
		}

		void Date_Click(object sender, EventArgs e)
		{
			DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
																 {
																	 mFilterTime = time;
																	 UpdateChart();
																	 mDateTextView.Text = time.ToString("MMMMMMMMM yyyy").ToUpper();
																 });
			frag.Show(FragmentManager, DatePickerFragment.TAG);
		}

		void UpdateChart()
		{
			//throw new NotImplementedException();
			List<Bill> mBills = new Decorators.DateFilter(mDBS.GetAllBills(), mFilterTime);
			mWebInterface.UpdateBills(mBills);
			mChartsView.LoadUrl(mChartURL);

			if (mBills == null || mBills.Count == 0)
			{
				mChartsView.Visibility = ViewStates.Gone;
			}
			else
			{
				mChartsView.Visibility = ViewStates.Visible;
			}
		}

		protected override void OnResume()
		{
			base.OnResume();

			//Initialize database service
			GenDataService mDBS = DataHelper.getInstance().getGenDataService();
			mDBS.CreateTableIfNotExists();

			List<Bill> bills = mDBS.GetAllBills();
			mCategories.Clear();
			foreach (Bill bill in bills)
			{
				if (!mCategories.Contains(bill.Category))
				{
					mCategories.Add(bill.Category);
				}
			}

			if (mCategories == null || mCategories.Count == 0)
			{
				mNoResultsText.Visibility = ViewStates.Visible;
				mNoResultsImage.Visibility = ViewStates.Visible;
			}

			mAdapter.update(mCategories);
			mCategoriesListview.Adapter = mAdapter;
		}
	}

	public class WebAppInterface : Java.Lang.Object
	{
		string[] Categories;
		List<Bill> bills;

		public WebAppInterface(string[] c)
		{
			Categories = c;
			bills = new List<Bill>();
		}

		[Export]
		[JavascriptInterface]
		public int getCategoriesCount()
		{
			return Categories.Count();
		}

		[Export]
		[JavascriptInterface]
		public string getName(int i)
		{
			return Categories[i];
		}

		[Export]
		[JavascriptInterface]
		public double getValue(int i)
		{
			return bills.Where(o => o.Category == Categories[i]).Sum(o => o.Amount);
		}

		public void UpdateBills(List<Bill> bills)
		{
			this.bills = bills;
		}

	}
}