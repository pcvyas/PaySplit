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

using PaySplit;

namespace PaySplit.Droid
{

	[Activity(Label = "Categories", MainLauncher = false, Icon = "@mipmap/ic_launcher")]
	public class CategoryActivity : Activity
	{
		private List<string> mCategories = new List<string>();
		private CategoryListViewAdapter mAdapter;

		private ListView mCategoriesListview;
		private ImageView mNoResultsImage;
		private TextView mNoResultsText;
		private static Spinner mLoadingSpinner;
		private static WebView mWebView;
		private WebViewClient mWebViewClient;

		private const string mChartURL = "file:///android_asset/pie_chart.html";
		private GenDataService mDBS;
		private WebView mChartsView;
		private WebAppInterface mWebInterface;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main_ListView);

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mNoResultsImage = FindViewById<ImageView>(Resource.Id.NoResultsImage);
			mCategoriesListview = FindViewById<ListView>(Resource.Id.View_ListView);
			mLoadingSpinner = FindViewById<Spinner>(Resource.Id.loadingSpinner);

			LayoutInflater layoutInflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
			View header = (View)layoutInflater.Inflate(Resource.Layout.DashboardLayout, null);
			mCategoriesListview.AddHeaderView(header);

			// Setup adapter
			mAdapter = new CategoryListViewAdapter(this, mCategories);
			mCategoriesListview.Adapter = mAdapter;

			mWebView = FindViewById<WebView>(Resource.Id.chartView);
			mWebViewClient = new MyWebViewClient();
			mWebView.SetWebViewClient(mWebViewClient);

			//Expense Chart Config
			mChartsView = FindViewById<WebView>(Resource.Id.chartView);
			mChartsView.Settings.JavaScriptEnabled = true;
			mDBS = DataHelper.getInstance().getGenDataService();

			mWebView.Post(() =>
			{
				mWebView.Settings.JavaScriptEnabled = true;
				mWebView.Settings.JavaScriptCanOpenWindowsAutomatically = false;

				mWebInterface = new WebAppInterface(Resources.GetStringArray(Resource.Array.categories_array));
				mWebInterface.UpdateBills(mDBS.GetAllBills());
				mChartsView.AddJavascriptInterface(mWebInterface, "Android");
				mChartsView.LoadUrl(mChartURL);

				mWebView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
				mWebView.SetLayerType(LayerType.Hardware, null);
				mWebView.Settings.CacheMode = CacheModes.NoCache;
				mWebView.Settings.JavaScriptEnabled = true;
			});
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

		private class MyWebViewClient : WebViewClient
		{
			public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
			{
				base.OnPageStarted(view, url, favicon);
				if (mLoadingSpinner != null)
				{
					mLoadingSpinner.Visibility = ViewStates.Visible;
				}
				if (mWebView != null)
				{
					mWebView.Visibility = ViewStates.Gone;
				}
			} 

			public override void OnPageFinished(WebView view, string url)
			{
				base.OnPageFinished(view, url);
				if (mLoadingSpinner != null)
				{
					mLoadingSpinner.Visibility = ViewStates.Gone;
				}
				if (mWebView != null)
				{
					mWebView.Visibility = ViewStates.Visible;
				}
			}
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