﻿using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.PM;
using Android.Provider;
using Java.IO;
using Android.App;
using Android.Widget;
using Android.Views;
using Android.Graphics;
namespace PaySplit.Droid
{
	public class CameraService
	{
		ImageView iw;
		Activity activity;
		string imagePath;

		public CameraService(ImageView imageView, Activity activity)
		{
			iw = imageView;
			this.activity = activity;
		}

		/*
		public void CreateDirectoryForPictures()
		{
			App.dir = new File(
				Android.OS.Environment.GetExternalStoragePublicDirectory(
					Android.OS.Environment.DirectoryPictures), "BillsImages");
			if (!App.dir.Exists())
			{
				App.dir.Mkdirs();
			}

		}*/

		private File CreateImageFile() 
		{
			// Create an image file name
	
		    string imageFileName = String.Format("Bill_{0}.jpg", Guid.NewGuid());
			File storageDir = Android.Support.V4.Content.ContextCompat.GetExternalFilesDirs(activity.BaseContext, Android.OS.Environment.DirectoryPictures)[0];
			App.file = File.CreateTempFile(
				imageFileName,  /* prefix */
				".jpg",         /* suffix */
				storageDir      /* directory */
			);

				// Save a file: path for use with ACTION_VIEW intents
			imagePath = App.file.AbsolutePath;
			return App.file;
		}

		public bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities =
				activity.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}

		public void TakeAPicture()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			App.file = CreateImageFile();
			Android.Net.Uri u = Android.Support.V4.Content.FileProvider.GetUriForFile(activity.BaseContext, "com.sag.paysplit.provider", App.file);
			intent.PutExtra(MediaStore.ExtraOutput, u);
			activity.StartActivityForResult(intent, 1);
		}

		public void SavePicture()
		{
			// Make it available in the gallery
			Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
			Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App.file);
			mediaScanIntent.SetData(contentUri);
			activity.SendBroadcast(mediaScanIntent);

			// Display in ImageView. We will resize the bitmap to fit the display.
			// Loading the full sized image will consume to much memory
			// and cause the application to crash.
			int height = activity.Resources.DisplayMetrics.HeightPixels;
			int width = activity.Resources.DisplayMetrics.WidthPixels;

			//App.bitmap = MediaStore.Images.Media.GetBitmap(activity.ContentResolver, contentUri);
			App.bitmap = App.file.Path.LoadAndResizeBitmap(width, height);
			if (App.bitmap != null)
			{
				iw.SetImageBitmap(App.bitmap);
				App.bitmap = null;

				iw.Visibility = ViewStates.Visible;
				// Dispose of the Java side bitmap.
				GC.Collect();
			}
			else
			{
				App.file = null;
			}

		}

		public string GetSavedPicturePath()
		{
			if (App.file != null)
				return App.file.Path;
			else
				return null;
		}

		public Bitmap GetSavedImage()
		{
			return App.bitmap;
		}

	}

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
			//Image not found
			if (resizedBitmap == null)
			{
				return null;
			}
		
			Matrix matrix = new Matrix();
			matrix.PostRotate(90);
			Bitmap rotatedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, true);

			return rotatedBitmap;
		}
	}


	public static class App
	{
		public static File file;
		public static File dir;
		public static Bitmap bitmap;
	}


}