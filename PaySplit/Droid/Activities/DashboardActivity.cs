
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
	[Activity(Label = "Dashboard", MainLauncher = false, Icon = "@mipmap/new_icon")]
	public class DashboardActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.DashboardLayout);

			string[] mCategories = Resources.GetStringArray(Resource.Array.categories_array);
			WebView wb = FindViewById<WebView>(Resource.Id.chartView);

			GenDataService mDBS = DataHelper.getInstance().getGenDataService();
		
			/*
			LayoutInflater layoutInflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
			View header = (View)layoutInflater.Inflate(Resource.Layout.DateFilterView, null);
			TextView mDateTextView = header.FindViewById<TextView>(Resource.Id.DateFilter_Date_TextView);
			mDateTextView.Text = DateTime.Now.ToString("MMMMMMMMM yyyy").ToUpper();
			*/
			//header.Click += Date_Click;
			// .AddHeaderView(header);

			// Create your application here

			wb.AddJavascriptInterface(new WebAppInterface(mCategories, mDBS), "Android");
			string chartURL = "file:///android_asset/pie_chart.html";
			wb.LoadUrl(chartURL);
			wb.Settings.JavaScriptEnabled = true;


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
