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
using com.refractored.fab;

using PaySplit;

namespace PaySplit.Droid
{

	[Activity(Label = "Categories", MainLauncher = false, Icon = "@mipmap/ic_launcher")]
	public class CategoryActivity : Activity
	{
		private List<string> mCategories = new List<string>();
		private CategoryListViewAdapter mAdapter;

		private ListView mCategoriesListview;
		private TextView mNoResultsText;
		private static Spinner mLoadingSpinner;
		private static WebView mWebView;
		private WebViewClient mWebViewClient;

		private const string mChartURL = "file:///android_asset/pie_chart.html";
		private GenDataService mDBS;
		private WebAppInterface mWebInterface;

		private TextView mDateTextView;
		private DateTime mFilterTime;

		private FloatingActionButton mFloatingActionButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main_ListView);

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mNoResultsText.Text = "As you create bills, categories will appear here. \nTapping on categories will allow you to view bills in that category.";

			mCategoriesListview = FindViewById<ListView>(Resource.Id.View_ListView);
			mLoadingSpinner = FindViewById<Spinner>(Resource.Id.loadingSpinner);

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

			mFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);

			// Setup adapter
			mAdapter = new CategoryListViewAdapter(this, mCategories);
			mCategoriesListview.Adapter = mAdapter;

			mWebView = FindViewById<WebView>(Resource.Id.chartView);
			mWebViewClient = new MyWebViewClient();
			mWebView.SetWebViewClient(mWebViewClient);

			//Get DBS
			mDBS = DataHelper.getInstance().getGenDataService();
			//Init WebInterface
			mWebInterface = new WebAppInterface(Resources.GetStringArray(Resource.Array.categories_array));

			mWebView.Post(() =>
			{
				mWebView.Settings.JavaScriptEnabled = true;
				mWebView.Settings.JavaScriptCanOpenWindowsAutomatically = false;
				mWebView.AddJavascriptInterface(mWebInterface, "Android");

				mWebView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
				mWebView.SetLayerType(LayerType.Hardware, null);
				mWebView.Settings.CacheMode = CacheModes.NoCache;
			});

			//Load the Charts
			UpdateChart();
		
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
			mWebView.LoadUrl(mChartURL);

			if (mBills == null || mBills.Count == 0)
			{
				mWebView.Visibility = ViewStates.Gone;
			}
			else
			{
				mWebView.Visibility = ViewStates.Visible;
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
				HideChart();
			}
			else
			{
				mNoResultsText.Visibility = ViewStates.Gone;
				ShowChart();
			}

			mAdapter.update(mCategories);
			mCategoriesListview.Adapter = mAdapter;
		}

		private void HideChart()
		{
			if (mLoadingSpinner != null)
			{
				mLoadingSpinner.Visibility = ViewStates.Gone;
			}
			if (mWebView != null)
			{
				mWebView.Visibility = ViewStates.Gone;
			}
		}

		private void ShowChart()
		{
			if (mLoadingSpinner != null)
			{
				mLoadingSpinner.Visibility = ViewStates.Visible;
			}
			if (mWebView != null)
			{
				mWebView.Visibility = ViewStates.Visible;
			}
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