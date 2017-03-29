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
            mNoResultsText.Text = "You currently have no contacts! Create contacts through the settings menu.\n";

            mTransactionsListview = FindViewById<ListView>(Resource.Id.View_ListView);
            mLoadingSpinner = FindViewById<Spinner>(Resource.Id.loadingSpinner);

            mFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
            mFloatingActionButton.Visibility = ViewStates.Gone;

            mDBS = DataHelper.getInstance().getGenDataService();
            mDBS.CreateTableIfNotExists();

            UpdateListView();
        }

        private void UpdateListView()
        {
            mContacts.Clear();
            mContacts = mDBS.GetAllContacts().Where(o => o.Id != 1).ToList();
            mTransactions = mDBS.GetAllTransactions();

            if (mContacts == null || mContacts.Count == 0)
            {
                mNoResultsText.Visibility = ViewStates.Visible;
            }
            else if (mTransactions == null || mTransactions.Count == 0)
            {
                mNoResultsText.Text = "You currently have no transactions! Transactions for contacts will be added here as bills are created and split.\n";
                mNoResultsText.Visibility = ViewStates.Visible;
            } else
            {
                // Find the balances for each contact
                minTransactions = findMinTransactionsForContacts(mContacts, mTransactions);

                if (minTransactions.Count == 0)
                {
                    mNoResultsText.Text = "All bills are settled up! No payments are needed.\n";
                    mNoResultsText.Visibility = ViewStates.Visible;
                } else
                {
                    mNoResultsText.Visibility = ViewStates.Gone;
                    // Setup adapter
                    mAdapter = new MinTransactionListViewAdapter(this, minTransactions);
                    mTransactionsListview.Adapter = mAdapter;
                }
            }
        }

        private List<Transaction> findMinTransactionsForContacts(List<Contact> contacts, List<Transaction> transactions)
        {
            Dictionary<string, double> balances = new Dictionary<string, double>();
            //Dictionary<string, double> givers = new Dictionary<string, double>();
            Queue<Tuple<string, double>> givers = new Queue<Tuple<string, double>>();
            //Dictionary<string, double> receivers = new Dictionary<string, double>();
            Queue<Tuple<string, double>> receivers = new Queue<Tuple<string, double>>();

            Contact currentUser = mDBS.getUserContactInformation();

            if (currentUser != null)
            {
                balances.Add(mDBS.getUserContactInformation().Email, 0);
            }

            foreach (Contact c in contacts)
            {
                balances.Add(c.Email, 0);
            }

            foreach (Transaction t in transactions)
            {
                if (t.Completed)
                {
                    continue;
                }

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
            }

            // Split up the contacts to givers and receivers
            foreach (KeyValuePair<string, double> balance in balances)
            {
                if (balance.Value > 0)
                {
                    //receivers.Add(balance.Key, balance.Value);
                    receivers.Enqueue(new Tuple<string, double>(balance.Key, balance.Value));
                }
                else if (balance.Value < 0)
                {
                    //givers.Add(balance.Key, balance.Value);
                    givers.Enqueue(new Tuple<string, double>(balance.Key, balance.Value));
                }
            }
            
            // Go through the list of givers and give each receiver what they need until
            // the giver has paid up their balance
            // When a receiver received everything they need, they are removed
            List<Transaction> netTransactions = new List<Transaction>();

            while (givers.Count != 0)
            {
                Tuple<string, double> giver = givers.Peek();
                double deficit = giver.Item2 * -1;

                while (receivers.Count != 0)
                {
                    if (deficit <= 0)
                    {
                        break;
                    }

                    Tuple<string, double> receiver = receivers.Peek();
                    double surplus = receiver.Item2;
                    Transaction tran = new Transaction();

                    if (surplus == deficit)
                    {
                        tran.Amount = surplus;
                        tran.ReceiverEmail = receiver.Item1;
                        tran.SenderEmail = giver.Item1;
                        deficit = 0;
                        surplus = 0;
                        receivers.Dequeue();
                        givers.Dequeue();
                    }
                    else if (surplus < deficit)
                    {
                        tran.Amount = surplus;
                        tran.ReceiverEmail = receiver.Item1;
                        tran.SenderEmail = giver.Item1;
                        deficit -= surplus;
                        surplus = 0;
                        receivers.Dequeue();
                    }
                    else
                    {
                        tran.Amount = deficit;
                        tran.ReceiverEmail = receiver.Item1;
                        tran.SenderEmail = giver.Item1;
                        surplus -= deficit;
                        deficit = 0;
                        givers.Dequeue();
                    }
                    netTransactions.Add(tran);
                }
            }

            //foreach (KeyValuePair<string, double> giver in givers)
            //{
            //    double deficit = giver.Value *-1;

            //    foreach (KeyValuePair<string, double> receiver in receivers)
            //    {
            //        if (deficit <= 0)
            //        {
            //            break;
            //        }

            //        double surplus = receiver.Value;
            //        Transaction tran = new Transaction();

            //        if (surplus <= deficit)
            //        {
            //            tran.Amount = surplus;
            //            tran.ReceiverEmail = receiver.Key;
            //            tran.SenderEmail = giver.Key;
            //            deficit -= surplus;
            //            receivers.Remove(receiver.Key);
            //        } else
            //        {
            //            tran.Amount = deficit;
            //            tran.ReceiverEmail = receiver.Key;
            //            tran.SenderEmail = giver.Key;
            //            surplus -= deficit;
            //            deficit = 0;
            //        }
            //        netTransactions.Add(tran);
            //    }
            //}
            return netTransactions;
        }
    }
}