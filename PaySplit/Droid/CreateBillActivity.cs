
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
	[Activity(Label = "Create Bill")]
	public class CreateBillActivity : Activity
	{

		ImageView iw;
		CameraService cs;
		Bill bill;
		GenDataService dbs;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.CreateBillEntry);
			// Create your application here


			//Bill
			bill = new Bill();

			//Init num picker
			NumberPicker np = FindViewById<NumberPicker>(Resource.Id.numPeople);
			np.MaxValue = 20;
			np.MinValue = 1;


			//Init Database
			DataHelper dbPath = new DataHelper();
			dbPath.CreateDataBase("PaySplitDataDb.db3");

			//Initialize database service
			dbs = new GenDataService(dbPath.DBPath);

			/**************************
			*  Take a Photo
			* ************************/
			iw = FindViewById<ImageView>(Resource.Id.picture);

			cs = new CameraService(iw, this);

			if (cs.IsThereAnAppToTakePictures())
			{
				cs.CreateDirectoryForPictures();

				ImageButton takePhoto = FindViewById<ImageButton>(Resource.Id.takePic);

				takePhoto.Click += delegate
				{
					cs.TakeAPicture();
					bill.ImagePath = cs.GetSavedPicturePath();
				};
			}

			iw.Visibility = ViewStates.Invisible;


			//Initialize Button Listeners
			ImageButton pickCategoryBtn = FindViewById<ImageButton>(Resource.Id.pickCat);
			pickCategoryBtn.Click += PickCategory_Clicked;

			ImageButton addPeopleBtn = FindViewById<ImageButton>(Resource.Id.pickPeople);
			addPeopleBtn.Click += AddPeople_Clicked;

			ImageButton saveBtn = FindViewById<ImageButton>(Resource.Id.save);
			saveBtn.Click += Save_Clicked;

			ImageButton cancelBtn = FindViewById<ImageButton>(Resource.Id.cancel);
			cancelBtn.Click += CancelBtn_Click;




		}

		void CancelBtn_Click(object sender, EventArgs e)
		{
			this.Finish();
		}

		void Save_Clicked(object sender, EventArgs e)
		{
			try
			{
				EditText name = FindViewById<EditText>(Resource.Id.name);
				bill.Name = name.Text;
				EditText description = FindViewById<EditText>(Resource.Id.description);
				bill.Description = description.Text;
				EditText amount = FindViewById<EditText>(Resource.Id.amount);
				bill.Amount = Double.Parse(amount.Text);

				bill.LastEdited = DateTime.Now;
				dbs.InsertBillEntry(bill);
				this.Finish();
			}
			catch (Exception exc)
			{
				Toast.MakeText(this, "Bill not saved!: " + exc.Message, ToastLength.Short).Show();
			}
		}

		void AddPeople_Clicked(object sender, EventArgs e)
		{
			
		}

		void PickCategory_Clicked(object sender, EventArgs e)
		{
			
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			cs.SavePicture();

		}
	}
}
