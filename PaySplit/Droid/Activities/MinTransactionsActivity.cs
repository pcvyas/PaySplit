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
    [Activity(Label = "Minimized Transactions")]
    public class MinTransactionsActivity : Activity
    {
        private GenDataService mDBS;
        Dictionary<string, double> mBalances = new Dictionary<string, double>();
        private List<Contact> mContacts = new List<Contact>();
        private List<Transaction> mTransactions = new List<Transaction>();
        private List<Transaction> minTransactions = new List<Transaction>();

        // Views
        private TextView mNoResultsText;
        private static Spinner mLoadingSpinner;
        private ListView mTransactionsListview;
        private MinTransactionListViewAdapter mAdapter;
        private FloatingActionButton mFloatingActionButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main_ListView);

            mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
            mNoResultsText.Text = "You currently have no contacts! Balances for contacts will be added here as bills are created.\n";

            mTransactionsListview = FindViewById<ListView>(Resource.Id.View_ListView);
            mLoadingSpinner = FindViewById<Spinner>(Resource.Id.loadingSpinner);

            mFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
            mFloatingActionButton.Visibility = ViewStates.Gone;

            mDBS = DataHelper.getInstance().getGenDataService();
            mDBS.CreateTableIfNotExists();

            mContacts = mDBS.GetAllContacts().Where(o => o.Id != 1).ToList();
            mTransactions = mDBS.GetAllTransactions();

            // Find the balances for each contact
            minTransactions = findMinTransactionsForContacts(mContacts, mTransactions);

            // Setup adapter
            mAdapter = new MinTransactionListViewAdapter(this, minTransactions);
            mTransactionsListview.Adapter = mAdapter;
        }

        private List<Transaction> findMinTransactionsForContacts(List<Contact> contacts, List<Transaction> transactions)
        {
            Dictionary<string, double> balances = new Dictionary<string, double>();
            Dictionary<string, double> givers = new Dictionary<string, double>();
            Dictionary<string, double> receivers = new Dictionary<string, double>();

            foreach (Contact c in contacts)
            {
                balances.Add(c.Email, 0);
            }

            foreach (Transaction t in transactions)
            {
                string payee = t.ReceiverEmail;
                string payer = t.SenderEmail;
                double amount = t.Amount;

                if (payee == payer)
                {
                    continue;
                }

                // Find each contact's net balance at the end of all transactions
                if (balances.ContainsKey(payee) && balances.ContainsKey(payer))
                {
                    balances[payee] += amount;
                    balances[payer] -= amount;
                }

                // Split up the contacts to givers and receivers
                foreach (KeyValuePair<string, double> balance in balances)
                {
                    if (balance.Value > 0)
                    {
                        receivers.Add(balance.Key, balance.Value);
                    }
                    else if (balance.Value < 0)
                    {
                        givers.Add(balance.Key, balance.Value);
                    }
                }
            }

            // Go through the list of givers and give each receiver what they need until
            // the giver has paid up their balance
            // When a receiver received everything they need, they are removed
            List<Transaction> netTransactions = new List<Transaction>();

            foreach (KeyValuePair<string, double> giver in givers)
            {
                double deficit = giver.Value *-1;

                foreach (KeyValuePair<string, double> receiver in receivers)
                {
                    if (deficit <= 0)
                    {
                        break;
                    }

                    double surplus = receiver.Value;
                    Transaction tran = new Transaction();

                    if (surplus <= deficit)
                    {
                        tran.Amount = surplus;
                        tran.ReceiverEmail = receiver.Key;
                        tran.SenderEmail = giver.Key;
                        deficit -= surplus;
                        receivers.Remove(receiver.Key);
                    } else
                    {
                        tran.Amount = deficit;
                        tran.ReceiverEmail = receiver.Key;
                        tran.SenderEmail = giver.Key;
                        surplus -= deficit;
                        deficit = 0;
                    }
                    netTransactions.Add(tran);
                }
            }
            return netTransactions;
        }
    }
}