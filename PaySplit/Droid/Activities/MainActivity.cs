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
		ImageView iw;
		CameraService cs;

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

			// TODO: set record_exists properly based on if onboarding has been completed
			bool user_info_recorded = true;
			if (!user_info_recorded)
			{
				setupDatabase();
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

		private void setupDatabase()
		{
			//Generate or Initialize Database Path
			DataHelper dbPath = new DataHelper();
			dbPath.CreateDataBase(Constants.PAYSPLIT_DB_NAME);

			//Initialize database service
			GenDataService dbs = new GenDataService(dbPath.DBPath);

			//Create Table
			dbs.CreateTable();
		}

		//To handle Camera action completed
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			cs.SavePicture();
		}
	}
}

