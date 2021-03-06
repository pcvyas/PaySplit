﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
	
	[Activity(Label = "My Contacts")]
	public class ViewContactsActivity : Activity
	{
		
		private GenDataService mDBS;
		private List<Contact> mContacts = new List<Contact>();

		// Views
		private TextView mNoResultsText;
		private ListView mContactsListview;
		private ContactsListViewAdapter mAdapter;
		private FloatingActionButton mFloatingActionButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main_ListView);

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mNoResultsText.Text = "You currently have no contacts! Contacts will be added here as bills are created.\nTap the + button to add contacts!";

			mContactsListview = FindViewById<ListView>(Resource.Id.View_ListView);

			mFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton);
			mFloatingActionButton.Visibility = ViewStates.Visible;
			mFloatingActionButton.Click += FAB_Click;

			mDBS = DataHelper.getInstance().getGenDataService();
			mDBS.CreateTableIfNotExists();

			mContacts = mDBS.GetAllContacts().Where(o => o.Id != 1).ToList();

			// Setup adapter
			mAdapter = new ContactsListViewAdapter(this, mContacts);
			mContactsListview.Adapter = mAdapter;

			this.ActionBar.SetDisplayHomeAsUpEnabled(true);
		}

		protected override void OnResume()
		{
			base.OnResume();
			reloadContacts();
		}

		private void reloadContacts()
		{
			mContacts.Clear();
			mContacts = mDBS.GetAllContacts().Where(o => o.Id != 1).ToList();
			mAdapter.update(mContacts);
			if (mContacts == null || mContacts.Count == 0)
			{
				mNoResultsText.Visibility = ViewStates.Visible;
			}
			else
			{
				mNoResultsText.Visibility = ViewStates.Gone;
			}
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

				if (!isValidEmail(email))
				{
					Toast.MakeText(this, "Invalid e-mail format, contact was not created.", ToastLength.Short).Show();
					return;
				}

				GenDataService dbs = DataHelper.getInstance().getGenDataService();
				Contact newContact = dbs.getContactByEmail(email);
				if (newContact == null)
				{
					dbs.InsertContactEntry(c);
				}
				else
				{
					Toast.MakeText(this, "Error: Contact with this e-mail address already exists.", ToastLength.Short).Show();
				}
				reloadContacts();
			});
			alertDialog.SetNegativeButton("Cancel", delegate {});
			AlertDialog dialog = alertDialog.Create();
			dialog.Show();
		}

		private bool isValidEmail(String email)
		{
			try
			{
				MailAddress mailAddress = new MailAddress(email);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
