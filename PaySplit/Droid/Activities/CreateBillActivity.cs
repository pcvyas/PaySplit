
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
	[Activity(Label = "Create Bill", MainLauncher = false, Icon = "@mipmap/new_icon")]
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

			// Initialize the Categories Spinner
			Spinner categoriesSpinner = FindViewById<Spinner>(Resource.Id.category_spinner);
			String[] categories = Resources.GetStringArray(Resource.Array.categories_array);
			ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, categories);
			categoriesSpinner.Adapter = adapter;

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

			Button saveBtn = FindViewById<Button>(Resource.Id.save);
			saveBtn.Click += Save_Clicked;

			Button cancelBtn = FindViewById<Button>(Resource.Id.cancel);
			cancelBtn.Click += CancelBtn_Click;

			TextView date = FindViewById<TextView>(Resource.Id.date);
			date.Text = bill.Date.ToLongDateString();
			date.Click += Date_Click;
		}

		void Date_Click(object sender, EventArgs e)
		{
			TextView date = FindViewById<TextView>(Resource.Id.date);
			DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
																 {
																	 date.Text = time.ToLongDateString();
																	 bill.Date = time;

																 });
			frag.Show(FragmentManager, DatePickerFragment.TAG);
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

				Spinner categoriesSpinner = FindViewById<Spinner>(Resource.Id.category_spinner);
				bill.Category = categoriesSpinner.SelectedItem.ToString();

				dbs.InsertBillEntry(bill);
				this.Finish();
			}
			catch (Exception exc)
			{
				Toast.MakeText(this, "Bill not saved!: " + exc.Message, ToastLength.Short).Show();
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			cs.SavePicture();

		}
	}


}
