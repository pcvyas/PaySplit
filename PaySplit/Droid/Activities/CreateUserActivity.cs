
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
		private EditText mPasswordEditTextView;

		private Button mStartButton;

		private GenDataService mDBS;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.CreateUser);

			mDBS = DataHelper.getInstance().getGenDataService();

			mNameEditTextView = FindViewById<EditText>(Resource.Id.Create_Name_EditText);
			mEmailEditTextView = FindViewById<EditText>(Resource.Id.Create_Email_EditText);
			mPasswordEditTextView = FindViewById<EditText>(Resource.Id.Create_Passwor_EditText);

			mStartButton = FindViewById<Button>(Resource.Id.Create_StartBtn);
			mStartButton.Click += delegate
			{
				String name = mNameEditTextView.Text;
				String email = mEmailEditTextView.Text;
				String password = mPasswordEditTextView.Text;
				if ((name == null || ("").Equals(name)) || (email == null || !isValidEmail(email)) || (password == null || ("").Equals(password) || password.Length < 10))
				{
					showErrorDialog();
				}
				else
				{
					// Create initial contact as user
					Contact c = new Contact();
					c.Id = 1;
					c.UID = Constants.MAIN_USER_DEFAULT_UID;
					c.FullName = name;
					c.Email = email;
					c.Password = password;
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
			alert.SetMessage("Please enter a valid name, e-mail address & password. Password length must be at least 10 characters.");
			alert.SetNegativeButton("Ok", (senderAlert, args) => {});
			Dialog dialog = alert.Create();
			dialog.Show();
		}
	}
}
