
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
	[Activity(Label = "Split The Bill")]
	public class SplitActivity : Activity
	{

		private GenDataService mDBS;
		private List<Contact> mContacts = new List<Contact>();

		private int total;
		private string billUID;

		bool splitSelectionScreen;
		bool splitAmountScreen;

		// Views
		private ListView mContactsListview;
		private SplitContactsListViewAdapter mAdapter;
		private SplitAmountAdapter sAdapter;
		private FloatingActionButton nextFloatingActionButton;
		private FloatingActionButton backFloatingActionButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SplitTemplate);

			mContactsListview = FindViewById<ListView>(Resource.Id.View_ListView);

			nextFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.nextButton);
			nextFloatingActionButton.Visibility = ViewStates.Visible;
			nextFloatingActionButton.Click += next_click;

			backFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.backButton);
			backFloatingActionButton.Click += back_click;

			mDBS = DataHelper.getInstance().getGenDataService();
			mContacts = mDBS.GetAllContacts();

			mContactsListview.Adapter = mAdapter;
			splitAmountScreen = false;
			splitSelectionScreen = true;

			billUID = Intent.GetStringExtra("uid");
			string ownerEmail = mDBS.getBillByUID(billUID).OwnerEmail;

			// Setup adapters
			mAdapter = new SplitContactsListViewAdapter(this, mContacts, ownerEmail);
			sAdapter = new SplitAmountAdapter(this, mContacts, total);

			total = Intent.GetIntExtra("amount", 0);
			sAdapter.setTotal(total);

			mContacts.Clear();
			mContacts = mDBS.GetAllContacts();
			if (mContacts == null || mContacts.Count <= 1)
			{
				Toast.MakeText(this, "You have no contacts to split this bill with! Please create contacts first in Settings.", ToastLength.Short).Show();
				Finish();
			}
			else
			{
				mAdapter.update(mContacts);
				//sAdapter.update(mContacts);
				mContactsListview.Adapter = mAdapter;
				//sContactsListview.Adapter = sAdapter;
			}

			this.ActionBar.SetDisplayHomeAsUpEnabled(true);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					ShowDiscardDialog();
					return true;
				default:
					return base.OnOptionsItemSelected(item);
			}
		}

		void ShowDiscardDialog()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle("Discard new bill?");
			alert.SetMessage("Are you sure you want to discard this bill? All unsaved changed will be lost.");
			alert.SetPositiveButton("Ok", (senderAlert, args) =>
			{
				mDBS.DeleteBill(mDBS.getBillByUID(billUID));
				this.Finish();
			});
			alert.SetNegativeButton("Cancel", (senderAlert, args) => { });
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		void back_click(object sender, EventArgs e)
		{

			if (splitAmountScreen)
			{
				mContactsListview.Adapter = mAdapter;
				splitSelectionScreen = true;
				splitAmountScreen = false;
				backFloatingActionButton.Visibility = ViewStates.Gone;
			}
			else
			{
				ShowDiscardDialog();
			}
		}

		void next_click(object sender, EventArgs e)
		{

			if (splitSelectionScreen)
			{
				sAdapter.update(mAdapter.getSelectedContacts());
				mContactsListview.Adapter = sAdapter;
				splitSelectionScreen = false;
				splitAmountScreen = true;
				backFloatingActionButton.Visibility = ViewStates.Visible;
			}
			else
			{
				double coveredAmount = sAdapter.getCoveredAmount();
				if (!coveredAmount.Equals(total))
				{
					ShowInvalidAmountsDialog(mAdapter.getSelectedContacts().Count, coveredAmount, total);
				}
				else
				{
					string ownerEmail = mDBS.getBillByUID(billUID).OwnerEmail;
					List<Contact> contacts = sAdapter.mContacts;
					List<double> amounts = sAdapter.mAmounts;

					List<Transaction> transactions = new List<Transaction>();
					for (int i = 0; i < contacts.Count; i ++)
					{
						Transaction t = new Transaction();
						t.BillUID = billUID;
						t.Amount = amounts[i];
						t.SenderEmail = contacts[i].Email;
						t.ReceiverEmail = ownerEmail;
						Debugger.Log("Created transaction for " + t.SenderEmail + " to " + t.ReceiverEmail + " [" + t.UID + "]");
						transactions.Add(t);
					}
					mDBS.InsertTransactionEntries(transactions);

					ShowSuccessSplitDialog();
				}
			}
		}

		void ShowInvalidAmountsDialog(int count, double cover, double total)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle("Invalid Split");
			alert.SetMessage("The current splits between the " + count + " people only sum up to $" + cover + " of the total $" +  total + "!");
			alert.SetPositiveButton("Fix", (senderAlert, args) =>
			{
			});
			alert.SetNegativeButton("Discard", (senderAlert, args) => {
				ShowDiscardDialog();
			});
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		void ShowSuccessSplitDialog()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle("Successfully Split");
			alert.SetMessage("Succesfully split the bill between contacts");
			alert.SetCancelable(false);
			alert.SetPositiveButton("Ok", (senderAlert, args) => {
				var activity = new Intent(this, typeof(BillDetailsActivity));
				activity.PutExtra("uid", billUID);
				StartActivity(activity);
				this.Finish();
			});

			alert.Create().Show();
		}


	}

	public class SplitContactListViewHolder : Java.Lang.Object
	{
		public TextView contactName;
		public TextView contactEmail;
		public CheckBox checkBox;

		public SplitContactListViewHolder(View view)
		{
			contactName = view.FindViewById<TextView>(Resource.Id.contactName);
			contactEmail = view.FindViewById<TextView>(Resource.Id.contactEmail);
			checkBox = view.FindViewById<CheckBox>(Resource.Id.checkBox);
		}
	}
}
