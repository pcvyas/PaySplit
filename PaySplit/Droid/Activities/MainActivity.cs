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

namespace PaySplit.Droid
{
	[Activity(Label = "PaySplit", MainLauncher = true, Icon = "@mipmap/new_icon")]
	public class MainActivity : Activity
	{
		private ImageView iw;
		private CameraService cs;

		private DataHelper mDataHelper;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);


			//Add Entry
			Button addB = FindViewById<Button>(Resource.Id.Main_AddEntry);
            addB.Click += delegate
			{            
				StartActivity(typeof(CreateBillActivity));
			};

			mDataHelper = DataHelper.getInstance();
			mDataHelper.getGenDataService().CreateTable();

			// TODO: set record_exists properly based on if onboarding has been completed
			bool user_info_recorded = true;
			if (!user_info_recorded)
			{
				// send user to create user page (so we can record their name, email, etc) on first use and
				// persist this data for later
			}
			else
			{
				// The user has already registered, so we can just send them to the main view Bills Activity
				StartActivity(typeof(ViewBillsActivity));
				Finish();
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
		}

		//To handle Camera action completed
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			cs.SavePicture();
		}
	}
}

