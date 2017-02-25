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
	[Activity(Label = "Categories", MainLauncher = false, Icon = "@mipmap/new_icon")]
	public class CategoryActivity : Activity
	{
		private List<Bill> bills;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.ViewCategories_ListView);

			//Generate or Initialize Database Path
			DataHelper dbPath = new DataHelper();
			dbPath.CreateDataBase("PaySplitDataDb.db3");

			//Initialize database service
			GenDataService dbs = new GenDataService(dbPath.DBPath);

			//Create Table
			dbs.CreateTable();

			bills = dbs.GetAllBills();

			List<String> categories = new List<String>();
			foreach (Bill bill in bills)
			{
				if (!categories.Contains(bill.Category))
				{
					categories.Add(bill.Category);
				}
			}

			// Instantiate the listview
			ListView viewBillsListview = FindViewById<ListView>(Resource.Id.View_ListView);

			// Custom adapter
			CategoryListViewAdapter adapter = new CategoryListViewAdapter(this, categories);

			viewBillsListview.Adapter = adapter;
		}
	}
}