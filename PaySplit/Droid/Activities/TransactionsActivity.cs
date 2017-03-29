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
using com.refractored.fab;

namespace PaySplit.Droid
{
	[Activity(Label = "Transactions", MainLauncher = false, Icon = "@mipmap/ic_launcher")]
	public class TransactionsActivity : Activity
	{
		private List<Transaction> mTransactions;
		private TransactionListViewAdapter mAdapter;
		private ListView mTransactionsListview;

		private TextView mNoResultsText;
		private FloatingActionButton mFloatingActionButton;

		private GenDataService mDBS;
		private string billUid;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main_ListView);

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mTransactionsListview = FindViewById<ListView>(Resource.Id.View_ListView);

			//Make FAB invisible
			mFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
			mFloatingActionButton.Visibility = ViewStates.Invisible;

			//Initialize database service
			mDBS = DataHelper.getInstance().getGenDataService();

			billUid = Intent.GetStringExtra("bill-uid");
		}

		protected override void OnResume()
		{
			base.OnResume();

			// Load all trans from database
			mTransactions = mDBS.getTransactionsForBill(billUid);

			if (mTransactions == null)
			{
				mNoResultsText.Text = "No transactions exist\nfor this bill.";
				mNoResultsText.Visibility = ViewStates.Visible;
			}
			else if (mTransactions.Count <= 1)
			{
				mNoResultsText.Text = "This bill is not\nshared with anyone.";
				mNoResultsText.Visibility = ViewStates.Visible;
			}
			else
			{
				// Setup adapter
				mAdapter = new TransactionListViewAdapter(this, mTransactions);
				mTransactionsListview.Adapter = mAdapter;
			}

			this.ActionBar.SetDisplayHomeAsUpEnabled(true);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					Finish();
					return true;
				default:
					return base.OnOptionsItemSelected(item);
			}
		}
	}

}