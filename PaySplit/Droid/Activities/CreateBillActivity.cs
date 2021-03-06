﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Android.Text;

namespace PaySplit.Droid
{
	[Activity(Label = "Create Bill", MainLauncher = false, Icon = "@mipmap/ic_launcher")]
	public class CreateBillActivity : Activity
	{

		private ImageView mImageView;
		private CameraService mCameraService;
		private Bill mBill;
		private GenDataService mDBService;

		private Spinner mCategoriesSpinner;
		private Spinner mOwnerSpinner;

		ArrayAdapter<String> mCategoriesSpinnerAdapter;
		ContactsSuggestionArrayAdapter mOwnerSpinnerAdapter;

		private EditText mNameEditText;
		private EditText mDescriptionEditText;
		private EditText mAmountEditText;

		private String[] mCategories;
		private List<Contact> mContacts;

		private int CAMERA_REQUEST_CODE = 1;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.CreateBillEntry);

			//Initialize database service
			mDBService = DataHelper.getInstance().getGenDataService();

			mBill = new Bill();

			mNameEditText = FindViewById<EditText>(Resource.Id.name);
			mDescriptionEditText = FindViewById<EditText>(Resource.Id.description);
			mAmountEditText = FindViewById<EditText>(Resource.Id.amount);

			mCategories = Resources.GetStringArray(Resource.Array.categories_array);

			mCategoriesSpinner = FindViewById<Spinner>(Resource.Id.category_spinner);
			mOwnerSpinner = FindViewById<Spinner>(Resource.Id.owner_spinner);

			mCategoriesSpinnerAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, mCategories);
			mCategoriesSpinner.Adapter = mCategoriesSpinnerAdapter;

			mContacts = mDBService.GetAllContacts();
			mOwnerSpinnerAdapter = new ContactsSuggestionArrayAdapter(this, mContacts);
			mOwnerSpinner.Adapter = mOwnerSpinnerAdapter;

			mImageView = FindViewById<ImageView>(Resource.Id.picture);

			mCameraService = new CameraService(mImageView, this);

			//Camera Permission

			//mCameraService.CreateDirectoryForPictures();

			ImageButton takePhoto = FindViewById<ImageButton>(Resource.Id.takePic);

			takePhoto.Click += delegate
			{
				if (Build.VERSION.SdkInt >= BuildVersionCodes.M && CheckSelfPermission(Manifest.Permission.Camera) != Permission.Granted )
				{

					RequestPermissions(new String[] { Manifest.Permission.Camera }, CAMERA_REQUEST_CODE);
				}
				else
				{
					if (mCameraService.IsThereAnAppToTakePictures())
					{
						mCameraService.TakeAPicture();
						mBill.ImagePath = mCameraService.GetSavedPicturePath();
					}
				}

			};

			mImageView.Visibility = ViewStates.Invisible;

			Button saveBtn = FindViewById<Button>(Resource.Id.save);
			saveBtn.Click += Save_Clicked;

			Button cancelBtn = FindViewById<Button>(Resource.Id.cancel);
			cancelBtn.Click += CancelBtn_Click;

			TextView dateV = FindViewById<TextView>(Resource.Id.date);
			dateV.Text = mBill.Date.ToLongDateString();
			dateV.Click += Date_Click;

			this.ActionBar.SetDisplayHomeAsUpEnabled(true);
		}


		public override void OnBackPressed()
		{
			ShowDiscardDialog();
		}


		void Date_Click(object sender, EventArgs e)
		{
			TextView dateV = FindViewById<TextView>(Resource.Id.date);
			DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
																 {
																	 dateV.Text = time.ToLongDateString();
																	 mBill.Date = time;

																 });
			frag.Show(FragmentManager, DatePickerFragment.TAG);
		}

		void CancelBtn_Click(object sender, EventArgs e)
		{
			ShowDiscardDialog();
		}

		void Save_Clicked(object sender, EventArgs e)
		{
			try
			{
				if (!TextUtils.IsEmpty(mNameEditText.Text) && !TextUtils.IsEmpty(mAmountEditText.Text))
				{
					mBill.Name = mNameEditText.Text;
					mBill.Description = mDescriptionEditText.Text;
					mBill.Amount = Double.Parse(mAmountEditText.Text);
					mBill.LastEdited = DateTime.Now;
					mBill.Category = mCategoriesSpinner.SelectedItem.ToString();
					mBill.OwnerEmail = mContacts[mOwnerSpinner.SelectedItemPosition].Email;

					mDBService.InsertBillEntry(mBill);

					ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
					string cat = sharedPreferences.GetString(mBill.Category, "0");
					double limit = Convert.ToDouble(cat);
					// if there was an entry in preferences for this category, check if exceeds limit
					if (!limit.Equals(0))
					{
						double total = 0;
						foreach (Bill b in mDBService.GetBillsByCategory(mBill.Category))
						{
							total += b.Amount;
						}

						if (total + mBill.Amount > limit)
						{
							ShowBudgetExceededDialog(mBill.Category, limit, total);
						}
						else if ((total + mBill.Amount + BillDetailsActivity.CATEGORY_LIMIT_WARNING_THRESHOLD > limit))
						{
							ShowApproachingBudgetDialog(mBill.Category, limit, total);
						}
					}
					else
					{
						ShowSplitBillDialog();
					}
				}
				else
				{
					ShowInvalidInformationDialog();
				}
			}
			catch (Exception)
			{
				ShowInvalidInformationDialog();
			}
		}

		void ShowSplitBillDialog()
		{
			AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
			alertDialog.SetTitle("Would you like to split this bill with others?");
			alertDialog.SetMessage("This bill has been successfully created! Would you like to split it with your PaySplit contacts?");
			alertDialog.SetPositiveButton("Split", delegate
			{
				var splitActivity = new Intent(this, typeof(SplitActivity));
				splitActivity.PutExtra("amount", Convert.ToDouble(mAmountEditText.Text));
				splitActivity.PutExtra("uid", mBill.UID);
				StartActivity(splitActivity);
				Finish();
			});
			alertDialog.SetNegativeButton("No", delegate
			{
				Transaction t = new Transaction();
				t.Amount = mBill.Amount;
				t.BillUID = mBill.UID;
				t.ReceiverEmail = mBill.OwnerEmail;
				t.SenderEmail = mBill.OwnerEmail;
				t.Completed = false;
				mDBService.InsertTransactionEntry(t);
				var activity = new Intent(this, typeof(BillDetailsActivity));
				activity.PutExtra("uid", t.BillUID);
				StartActivity(activity);
				this.Finish();
			});

			alertDialog.Create().Show();
		}

		void ShowInvalidInformationDialog()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle("Missing fields");
			alert.SetMessage("Please make sure you enter all required fields before saving bill.");
			alert.SetPositiveButton("Ok", (senderAlert, args) => {});
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		void ShowDiscardDialog()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle("Discard bill?");
			alert.SetMessage("Are you sure you want to discard this bill? All unsaved data will be lost.");
			alert.SetPositiveButton("Ok", (senderAlert, args) => {
				this.Finish();
			});
			alert.SetNegativeButton("Cancel", (senderAlert, args) => {});
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		void ShowApproachingBudgetDialog(string billCat, double limit, double total)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle("Approaching Monthly Limit");
			alert.SetMessage("You're approaching your monthly budget for " + billCat + ". You've spent $" + total + " of your limit of $" + limit + "!");
			alert.SetPositiveButton("Ok", (senderAlert, args) => { 
				ShowSplitBillDialog();
			});
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		void ShowBudgetExceededDialog(string billCat, double limit, double total)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle("Montly Limit Exceeded");
			alert.SetMessage("You've exceeded your monthly budget for " + billCat + ". You've spent $" + total + " of your limit of $" + limit + "!");
			alert.SetPositiveButton("Ok", (senderAlert, args) => { 
				ShowSplitBillDialog();
			});
			Dialog dialog = alert.Create();
			dialog.Show();
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					Finish();
					return true;
				default:
					return base.OnOptionsItemSelected(item);
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok && requestCode == CAMERA_REQUEST_CODE)
			{
				mCameraService.SavePicture();
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions,  Permission[] grantResults)
		{
			if (requestCode == CAMERA_REQUEST_CODE)
			{
				if (grantResults[0] == Permission.Granted)
				{
					// Now user should be able to use camera
					if (mCameraService.IsThereAnAppToTakePictures())
					{
						mCameraService.TakeAPicture();
						mBill.ImagePath = mCameraService.GetSavedPicturePath();
					}
				}
				else
				{
					// Your app will not have this permission. Turn off all functions 
					// that require this permission or it will force close
				}
			}
		}
	}


}
