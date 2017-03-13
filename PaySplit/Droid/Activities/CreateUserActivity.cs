
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
	[Activity(Label = "CreateUserActivity", MainLauncher = false, Icon = "@mipmap/new_icon")]
	public class CreateUserActivity : Activity
	{

		private Button mStartButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.CreateUser);

			mStartButton = FindViewById<Button>(Resource.Id.Create_StartBtn);
			mStartButton.Click += delegate
			{
				// TODO: validate the data in the fields and store it in the users database
				StartActivity(typeof(ViewBillsActivity));
				Finish();
			};
		}

		protected override void OnResume()
		{
			base.OnResume();
			ActionBar.Hide();
		}
	}
}
