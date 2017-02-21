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
	[Activity(Label = "PaySplit", MainLauncher = true, Icon = "@mipmap/new_icon", Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
	public class MainActivity : Activity
	{
		int count = 1;
		ImageView iw;
		CameraService cs;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.CreateEntry);


			//Generate or Initialize Database Path
			DataHelper dbPath = new DataHelper();
			dbPath.CreateDataBase("PaySplitDataDb.db3");

			//Initialize database service
			GenDataService dbs = new GenDataService(dbPath.DBPath);

			//Create Table
			dbs.CreateTable();

			//Add Entry
			Button button = FindViewById<Button>(Resource.Id.AddEntry);
			button.Click += delegate
			{
				Bill b = new Bill() { Name = "Car Gas", Amount = 15.67, Description = "to ottawa" };
				dbs.InsertBillEntry(b);

			};

			//View Entry
			Button viewB = FindViewById<Button>(Resource.Id.Viewbtn);
			viewB.Click += delegate
			{
				var bills = dbs.GetAllBills();
				string s = "";
				foreach (var bill in bills)
				{
					s += bill.Name + "\n";
				}
				Toast.MakeText(this, s, ToastLength.Short).Show();

			};


			/**************************
			 *  Take a Photo
			 * ************************/
			iw = FindViewById<ImageView>(Resource.Id.imageView);

		    cs = new CameraService(iw, this);

			if (cs.IsThereAnAppToTakePictures())
			{
				cs.CreateDirectoryForPictures();

				Button takePhoto = FindViewById<Button>(Resource.Id.picture);

				takePhoto.Click += delegate 
				{
					cs.TakeAPicture();
				};
			}

			iw.Visibility = ViewStates.Invisible;

		}

		//To handle Camera action completed
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			cs.SavePicture();

		}

	}
}

