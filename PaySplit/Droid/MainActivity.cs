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
		int count = 1;
		ImageView iw;
		CameraService cs;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
            // Currently set to "CreateEntry", change to main dashboard page
			SetContentView(Resource.Layout.Main);


			//Generate or Initialize Database Path
			DataHelper dbPath = new DataHelper();
			dbPath.CreateDataBase("PaySplitDataDb.db3");

			//Initialize database service
			GenDataService dbs = new GenDataService(dbPath.DBPath);

			//Create Table
			dbs.CreateTable();

			//Add Entry
			Button addB = FindViewById<Button>(Resource.Id.Main_AddEntry);
            addB.Click += delegate
			{            
				StartActivity(typeof(CreateBillActivity));
			};

            ////View Entry
            Button viewB = FindViewById<Button>(Resource.Id.Main_Viewbtn);
            viewB.Click += delegate
            {
                StartActivity(typeof(ViewBillsActivity));
                //viewB.Click += delegate
                //{
                //	var bills = dbs.GetAllBills();
                //	string s = "";
                //	foreach (var bill in bills)
                //	{
                //		s += bill.Name + "\n";
                //	}
                //	Toast.MakeText(this, s, ToastLength.Short).Show();

                //};
            };

			//Add Entry
			Button categoriesBtn = FindViewById<Button>(Resource.Id.Main_Categoriesbtn);
			categoriesBtn.Click += delegate
			{
				StartActivity(typeof(CategoryActivity));
			};

            // Delete all bills (for testing)
            Button delB = FindViewById<Button>(Resource.Id.Main_DelBtn);
            delB.Click += delegate
            {
                dbs.deleteAllBills();
                Toast.MakeText(this, "Bills Deleted", ToastLength.Short).Show();
            };

            /**************************
			 *  Take a Photo
			 * ************************/
            iw = FindViewById<ImageView>(Resource.Id.Main_imageView);

		    cs = new CameraService(iw, this);

			if (cs.IsThereAnAppToTakePictures())
			{
				cs.CreateDirectoryForPictures();

				Button takePhoto = FindViewById<Button>(Resource.Id.Main_picture);

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

