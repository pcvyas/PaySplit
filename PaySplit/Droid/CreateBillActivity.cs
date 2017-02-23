
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
	[Activity(Label = "CreateBillActivity")]
	public class CreateBillActivity : Activity
	{

		ImageView iw;
		CameraService cs;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.CreateBillEntry);
			// Create your application here

			NumberPicker np = FindViewById<NumberPicker>(Resource.Id.numPeople);
			np.MaxValue = 20;
			np.MinValue = 1;


			/**************************
			*  Take a Photo
			* ************************/
			iw = FindViewById<ImageView>(Resource.Id.picture);

			cs = new CameraService(iw, this);

			if (cs.IsThereAnAppToTakePictures())
			{
				cs.CreateDirectoryForPictures();

				Button takePhoto = FindViewById<Button>(Resource.Id.takePic);

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
	}
}
