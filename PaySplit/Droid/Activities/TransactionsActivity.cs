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

			// Load all trans from database
			mTransactions = mDBS.GetAllTransactions();

			// Setup adapter
			mAdapter = new TransactionListViewAdapter(this, mTransactions);
			mTransactionsListview.Adapter = mAdapter;
		}

		protected override void OnResume()
		{
			base.OnResume();
			UpdateListView();
		}

		private void UpdateListView()
		{
			mTransactions = mDBS.GetAllTransactions();

			// we filter the bills using filter decorators

			if (mTransactions == null || mTransactions.Count == 0)
			{
				mNoResultsText.Visibility = ViewStates.Visible;
			}
			else
			{
				mNoResultsText.Visibility = ViewStates.Gone;
			}
			mAdapter.update(mTransactions);
			mTransactionsListview.Adapter = mAdapter;
		}
	}

}