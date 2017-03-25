
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
		private List<Contact> chosen = new List<Contact>();
		private List<Boolean> mChecked = new List<Boolean>();

		private int total;
		bool splitSelectionScreen;
		bool splitAmountScreen;
		bool splitAmountAdapter;

		// Views
		private TextView mNoResultsText;
		private static Spinner mLoadingSpinner;
		private ListView mContactsListview;
		private ListView sContactsListview;
		private SplitContactsListViewAdapter mAdapter;
		private SplitAmountAdapter sAdapter;
		private FloatingActionButton mFloatingActionButton;
		private FloatingActionButton nextFloatingActionButton;
		private FloatingActionButton backFloatingActionButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SplitTemplate);
			System.Diagnostics.Debug.WriteLine("gets in onresume");

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mNoResultsText.Text = "You currently have no contacts! Contacts will be added here as bills are created.\nTap the + button to add contacts!";

			mContactsListview = FindViewById<ListView>(Resource.Id.View_ListView);
			//sContactsListview = FindViewById<ListView>(Resource.Id.View_ListView);
			mLoadingSpinner = FindViewById<Spinner>(Resource.Id.loadingSpinner);

			mFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
			mFloatingActionButton.Visibility = ViewStates.Visible;
			mFloatingActionButton.Click += FAB_Click;

			nextFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.nextButton);
			nextFloatingActionButton.Visibility = ViewStates.Visible;
			nextFloatingActionButton.Click += next_click;

			backFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.backButton);
			backFloatingActionButton.Visibility = ViewStates.Visible;
			backFloatingActionButton.Click += back_click;

			mDBS = DataHelper.getInstance().getGenDataService();
			mContacts = mDBS.GetAllContacts();

			// Setup adapter
			mAdapter = new SplitContactsListViewAdapter(this, mContacts);
			mContactsListview.Adapter = mAdapter;
			splitAmountScreen = false;
			splitSelectionScreen = true;
			splitAmountAdapter = false;
			mChecked = mAdapter.getMChecked();



			total = Intent.GetIntExtra("amount", 0);
			this.ActionBar.SetDisplayHomeAsUpEnabled(true);
		}

		protected override void OnResume()
		{
			base.OnResume();
			System.Diagnostics.Debug.WriteLine("gets in onresume");
			//Initialize database service
			GenDataService mDBS = DataHelper.getInstance().getGenDataService();
			mDBS.CreateTableIfNotExists();

			mContacts.Clear();
			mContacts = mDBS.GetAllContacts();
			mContacts.RemoveAt(0);
			if (mContacts == null || mContacts.Count == 0)
			{
				mNoResultsText.Visibility = ViewStates.Visible;
			}
			else
			{
				mNoResultsText.Visibility = ViewStates.Gone;
			}

			mAdapter.update(mContacts, mChecked);
			//sAdapter.update(mContacts);
			mContactsListview.Adapter = mAdapter;
			//sContactsListview.Adapter = sAdapter;
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

		void FAB_Click(object sender, EventArgs e)
		{
			showCreateContactDialog();
		}

		void back_click(object sender, EventArgs e)
		{

			if (splitAmountScreen)
			{
				
				mContactsListview.Adapter = mAdapter;
				mAdapter.allContacts.Clear();
				for (int i = 0; i < mAdapter.allContacts.Count; i++)
				{	if (mChecked[i])
					{
						mAdapter.allContacts[i].checkBox.Checked = true;
					}
				}
				splitSelectionScreen = true;
				splitAmountScreen = false;
			}
			else
			{
				this.Finish();
				splitSelectionScreen = false;
				splitAmountScreen = false;
			}
		}

		void next_click(object sender, EventArgs e)
		{

			if (splitSelectionScreen)
			{
				chosen.Clear();
				for (int i = 0; i < mAdapter.Count; i++)
				{
					if (mAdapter.allContacts[i].checkBox.Checked)
					{
						chosen.Add(mContacts[i]);
						mChecked[i] = true;
					}

				}
				if (splitAmountAdapter == false)
				{
					sAdapter = new SplitAmountAdapter(this, chosen, total);
					splitAmountAdapter = true;
				}

				sAdapter.update(chosen);
				mContactsListview.Adapter = sAdapter;
				splitSelectionScreen = false;
				splitAmountScreen = true;
			}
			else
			{
				string ownerAmount = ((double)total / (chosen.Count + 1)).ToString("#.##");
				System.Diagnostics.Debug.WriteLine(ownerAmount);
				var splitAmount = new Intent();
				splitAmount.PutExtra("amount", ownerAmount);
				SetResult(Result.Ok, splitAmount);
				this.Finish();

			}




		}

		private void showCreateContactDialog()
		{
			AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
			alertDialog.SetTitle("Create New Contact");
			TextView nameTextView = new TextView(this);
			nameTextView.Text = "Name:";
			EditText nameEditText = new EditText(this);
			nameEditText.SetSingleLine(true);

			TextView emailTextView = new TextView(this);
			emailTextView.Text = "E-mail:";
			EditText emailEditText = new EditText(this);
			emailEditText.SetSingleLine(true);

			LinearLayout ll = new LinearLayout(this);
			ll.Orientation = Orientation.Vertical;
			ll.AddView(nameTextView);
			ll.AddView(nameEditText);
			ll.AddView(emailTextView);
			ll.AddView(emailEditText);
			ll.SetPadding(25, 25, 25, 25);
			alertDialog.SetView(ll);

			alertDialog.SetCancelable(false);
			alertDialog.SetPositiveButton("Create", delegate
			{
				string name = nameEditText.Text;
				string email = emailEditText.Text;
				Contact c = new Contact();
				c.FullName = name;
				c.Email = email;
				DataHelper.getInstance().getGenDataService().InsertContactEntry(c);
				mChecked.Add(false);
			});
			alertDialog.SetNegativeButton("Cancel", delegate
			{
				// TODO
			});

			AlertDialog dialog = alertDialog.Create();
			dialog.Show();
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
