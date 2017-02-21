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

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			cs.SavePicture();

		}
		/*
		private void CreateDirectoryForPictures()
		{
			App.dir = new File(
				Android.OS.Environment.GetExternalStoragePublicDirectory(
					Android.OS.Environment.DirectoryPictures), "BillsImages");
			if (!App.dir.Exists())
			{
				App.dir.Mkdirs();
			}
		}

		private bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities =
				PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}

		private void TakeAPicture(object sender, EventArgs eventArgs)
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			App.file = new File(App.dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
			intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App.file));
			StartActivityForResult(intent, 0);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			// Make it available in the gallery

			Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
			Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App.file);
			mediaScanIntent.SetData(contentUri);
			SendBroadcast(mediaScanIntent);

			// Display in ImageView. We will resize the bitmap to fit the display.
			// Loading the full sized image will consume to much memory
			// and cause the application to crash.

			int height = Resources.DisplayMetrics.HeightPixels;
			int width = iw.Height;
			App.bitmap = App.file.Path.LoadAndResizeBitmap(width, height);
			if (App.bitmap != null)
			{
				iw.SetImageBitmap(App.bitmap);
				App.bitmap = null;

				iw.Visibility = ViewStates.Visible;
				// Dispose of the Java side bitmap.
				GC.Collect();
			}

		}*/
	}
	/*
		public static class BitmapHelpers
		{
			public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
			{
				// First we get the the dimensions of the file on disk
				BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
				BitmapFactory.DecodeFile(fileName, options);

				// Next we calculate the ratio that we need to resize the image by
				// in order to fit the requested dimensions.
				int outHeight = options.OutHeight;
				int outWidth = options.OutWidth;
				int inSampleSize = 1;

				if (outHeight > height || outWidth > width)
				{
					inSampleSize = outWidth > outHeight
									   ? outHeight / height
									   : outWidth / width;
				}

				// Now we will load the image and have BitmapFactory resize it for us.
				options.InSampleSize = inSampleSize;
				options.InJustDecodeBounds = false;
				Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

				return resizedBitmap;
			}
		}


		public static class App
		{
			public static File file;
			public static File dir;
			public static Bitmap bitmap;
	    }
     */
	

}

