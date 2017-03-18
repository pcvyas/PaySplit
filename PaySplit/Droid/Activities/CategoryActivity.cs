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

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main_ListView);

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mNoResultsImage = FindViewById<ImageView>(Resource.Id.NoResultsImage);
			mCategoriesListview = FindViewById<ListView>(Resource.Id.View_ListView);

			LayoutInflater layoutInflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
			View header = (View)layoutInflater.Inflate(Resource.Layout.DashboardLayout, null);
			mCategoriesListview.AddHeaderView(header);

			// Setup adapter
			mAdapter = new CategoryListViewAdapter(this, mCategories);
			mCategoriesListview.Adapter = mAdapter;

			WebView wb = FindViewById<WebView>(Resource.Id.chartView);

			GenDataService mDBS = DataHelper.getInstance().getGenDataService();

			wb.AddJavascriptInterface(new WebAppInterface(Resources.GetStringArray(Resource.Array.categories_array), mDBS), "Android");
			string chartURL = "file:///android_asset/pie_chart.html";
			wb.LoadUrl(chartURL);
			wb.Settings.JavaScriptEnabled = true;
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
		GenDataService database;

		public WebAppInterface(string[] c, GenDataService dbs)
		{
			Categories = c;
			database = dbs;
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
			return database.GetAllBills().Where(o => o.Category == Categories[i]).Sum(o => o.Amount);
		}

	}
}