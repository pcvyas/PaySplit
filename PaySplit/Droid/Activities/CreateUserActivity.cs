
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PaySplit.Droid
{
	[Activity(Label = "CreateUserActivity", MainLauncher = false, Icon = "@mipmap/ic_launcher")]
	public class CreateUserActivity : Activity
	{
		private EditText mNameEditTextView;
		private EditText mEmailEditTextView;

		private Button mStartButton;

		private GenDataService mDBS;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.CreateUser);

			mDBS = DataHelper.getInstance().getGenDataService();

			mNameEditTextView = FindViewById<EditText>(Resource.Id.Create_Name_EditText);
			mEmailEditTextView = FindViewById<EditText>(Resource.Id.Create_Email_EditText);

			mStartButton = FindViewById<Button>(Resource.Id.Create_StartBtn);
			mStartButton.Click += delegate
			{
				String name = mNameEditTextView.Text;
				String email = mEmailEditTextView.Text;
				if ((name == null || ("").Equals(name)) || (email == null || !isValidEmail(email)))
				{
					showErrorDialog();
				}
				else
				{
					// Create initial contact as user
					Contact c = new Contact();
					c.Id = 1;
					c.FullName = name;
					c.Email = email;
					mDBS.InsertContactEntry(c);

					Settings.setUserCreated(this, true);

					// Start the View Bills Activity
					StartActivity(typeof(ViewBillsActivity));
					Finish();
				}
			};
		}

		protected override void OnResume()
		{
			base.OnResume();
			ActionBar.Hide();
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

		private void showErrorDialog()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle("Invalid Info");
			alert.SetMessage("Please enter a valid name & e-mail address to begin.");
			alert.SetNegativeButton("Ok", (senderAlert, args) => {});
			Dialog dialog = alert.Create();
			dialog.Show();
		}
	}
}
