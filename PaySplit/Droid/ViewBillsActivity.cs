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
    [Activity(Label = "View Bill", MainLauncher = false, Icon = "@mipmap/new_icon", Theme = "@android:style/Theme.Material.Light")]
    public class ViewBillsActivity : Activity
    {
        private List<Bill> bills;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewBills_ListView);

            //Generate or Initialize Database Path
            DataHelper dbPath = new DataHelper();
            dbPath.CreateDataBase("PaySplitDataDb.db3");

            //Initialize database service
            GenDataService dbs = new GenDataService(dbPath.DBPath);

            //Create Table
            dbs.CreateTable();

            bills = dbs.GetAllBills();

			string category = Intent.GetStringExtra("category");
			bills = fetchBillsByCategory(category);

            // Instantiate the listview
            ListView viewBillsListview = FindViewById<ListView>(Resource.Id.View_ListView);

            // Create the adapter to format the list

            // simple text adapter
            //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, billNames);

            // Custom adapter
            ListViewAdapter adapter = new ListViewAdapter(this, bills);

            viewBillsListview.Adapter = adapter;
        }

		private List<Bill> fetchBillsByCategory(String category)
		{
			if (category == null || category.Equals(""))
			{
				return bills;
			}

			foreach (Bill bill in bills)
			{
				if (bill.Category != category)
				{
					bills.Remove(bill);
				}
			}

			return bills;
		}
    }
}