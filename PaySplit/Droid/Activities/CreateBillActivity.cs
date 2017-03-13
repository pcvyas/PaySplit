
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

		private ImageView mImageView;
		private CameraService mCameraService;
		private Bill mBill;
		private GenDataService mDBService;

		private Spinner mCategoriesSpinner;

		ArrayAdapter<String> mAdapter;
		private String[] mCategories;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.CreateBillEntry);

			mBill = new Bill();

			mCategories = Resources.GetStringArray(Resource.Array.categories_array);

			mCategoriesSpinner = FindViewById<Spinner>(Resource.Id.category_spinner);

			mAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, mCategories);
			mCategoriesSpinner.Adapter = mAdapter;

			//Initialize database service
			mDBService = DataHelper.getInstance().getGenDataService();

			mImageView = FindViewById<ImageView>(Resource.Id.picture);

			mCameraService = new CameraService(mImageView, this);

			if (mCameraService.IsThereAnAppToTakePictures())
			{
				mCameraService.CreateDirectoryForPictures();

				ImageButton takePhoto = FindViewById<ImageButton>(Resource.Id.takePic);

				takePhoto.Click += delegate
				{
					mCameraService.TakeAPicture();
					mBill.ImagePath = mCameraService.GetSavedPicturePath();
				};
			}

			mImageView.Visibility = ViewStates.Invisible;

			Button saveBtn = FindViewById<Button>(Resource.Id.save);
			saveBtn.Click += Save_Clicked;

			Button cancelBtn = FindViewById<Button>(Resource.Id.cancel);
			cancelBtn.Click += CancelBtn_Click;

			TextView date = FindViewById<TextView>(Resource.Id.date);
			date.Text = mBill.Date.ToLongDateString();
			date.Click += Date_Click;
		}

		void Date_Click(object sender, EventArgs e)
		{
			TextView date = FindViewById<TextView>(Resource.Id.date);
			DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
																 {
																	 date.Text = time.ToLongDateString();
																	 mBill.Date = time;

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
				mBill.Name = name.Text;
				EditText description = FindViewById<EditText>(Resource.Id.description);
				mBill.Description = description.Text;
				EditText amount = FindViewById<EditText>(Resource.Id.amount);
				mBill.Amount = Double.Parse(amount.Text);

				mBill.LastEdited = DateTime.Now;

				Spinner categoriesSpinner = FindViewById<Spinner>(Resource.Id.category_spinner);
				mBill.Category = categoriesSpinner.SelectedItem.ToString();

				mDBService.InsertBillEntry(mBill);
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
			mCameraService.SavePicture();

		}
	}


}
