using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Content;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Provider;

using Java.IO;
using Android.Net;
using System;
using Android.Views;
using System.IO;

namespace PaySplit.Droid
{
	[Activity(Label = "PaySplit", MainLauncher = true, Icon = "@mipmap/new_icon")]
	public class MainActivity : Activity
	{

		private DataHelper mDataHelper;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.CreateUser);

			mDataHelper = DataHelper.getInstance();
			mDataHelper.getGenDataService().CreateTableIfNotExists();

			bool user_info_recorded = Settings.getUserCreated(this);
			if (!user_info_recorded)
			{
				// send user to create user page (so we can record their name, email, etc) on first use and
				// persist this data for later
				StartActivity(typeof(CreateUserActivity));
				Finish();
			}
			else
			{
				// The user has already registered, so we can just send them to the main view Bills Activity
				StartActivity(typeof(ViewBillsActivity));
				Finish();
			}
		}
	}
}