using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.PM;
using Android.Provider;
using Java.IO;
using Android.App;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Android.Content.Res;

namespace PaySplit.Droid
{
	public class CameraService 
	{
		ImageView iw;
		Activity activity;

		public CameraService(ImageView imageView, Activity activity)
		{
			iw = imageView;
			this.activity = activity;
		}
		public void CreateDirectoryForPictures()
		{
			App.dir = new File(
				Android.OS.Environment.GetExternalStoragePublicDirectory(
					Android.OS.Environment.DirectoryPictures), "BillsImages");
			if (!App.dir.Exists())
			{
				App.dir.Mkdirs();
			}
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
			App.file = new File(App.dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
			intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App.file));
			activity.StartActivityForResult(intent, 0);
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

		}

		public string GetSavedPicturePath()
		{
			if (App.bitmap != null)
			{
				return App.file.Path;
			}
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

			return resizedBitmap;
		}
	}


	public static class App
	{
		public static File file;
		public static File dir;
		public static Bitmap bitmap;
	}


}
