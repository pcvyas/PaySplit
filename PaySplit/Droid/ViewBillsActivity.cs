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
    [Activity(Label = "View Bills", MainLauncher = false, Icon = "@mipmap/new_icon")]
    public class ViewBillsActivity : Activity
    {
		private List<Bill> mBills = new List<Bill>();
		private BillListViewAdapter mAdapter;
		private ListView mViewBillsListview;

		private ImageView mNoResultsImage;
		private TextView mNoResultsText;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main_ListView);

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mNoResultsImage = FindViewById<ImageView>(Resource.Id.NoResultsImage);
            mViewBillsListview = FindViewById<ListView>(Resource.Id.View_ListView);

			// Setup adapter
            mAdapter = new BillListViewAdapter(this, mBills);
            mViewBillsListview.Adapter = mAdapter;
        }

		protected override void OnResume()
		{
			base.OnResume();

			//Generate or Initialize Database Path
			DataHelper dbPath = new DataHelper();
			dbPath.CreateDataBase(Constants.PAYSPLIT_DB_NAME);

			//Initialize database service
			GenDataService dbs = new GenDataService(dbPath.DBPath);

			//Create Table
			dbs.CreateTable();

			mBills = dbs.GetAllBills();

			string category = Intent.GetStringExtra(Constants.CATEGORY_EXTRA);
			mBills = fetchBillsByCategory(category);

			if (mBills == null || mBills.Count == 0)
			{
				mNoResultsText.Visibility = ViewStates.Visible;
				mNoResultsImage.Visibility = ViewStates.Visible;
			}

			mAdapter.update(mBills);
			mViewBillsListview.Adapter = mAdapter;
		}

		private List<Bill> fetchBillsByCategory(String category)
		{
			if (category == null || category.Equals(""))
			{
				return mBills;
			}

			List<Bill> filteredBills = new List<Bill>();
			foreach (Bill bill in mBills)
			{
				if (category.Equals(bill.Category))
				{
					filteredBills.Add(bill);
				}
			}

			return filteredBills;
        }
    }
}