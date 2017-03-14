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
    [Activity(Label = "Bill Details", MainLauncher = false, Icon = "@mipmap/new_icon")]
    public class BillDetailsActivity : Activity
    {
		
		private Bill mBill;
		private GenDataService mDBS;

		private TextView name;
		private TextView amount;
		private TextView date;
		private TextView category;
		private TextView desc;
		private ImageView image;
		private TextView updated;
		private Button editButton;

		private EditText name_edit;
		private EditText amount_edit;
		private EditText date_edit;
		private Spinner category_edit;
		private EditText desc_edit;
		private Button saveButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BillDetails);

            //string id = Intent.GetStringExtra("id") ?? "Data not available";
            string id = Intent.GetStringExtra("id");

			//Initialize database service
			mDBS = DataHelper.getInstance().getGenDataService();
            mBill = mDBS.getBillById(Int32.Parse(id));

            // Instatiate views
            name = FindViewById<TextView>(Resource.Id.Details_BillName);
            amount = FindViewById<TextView>(Resource.Id.Details_BillAmount);
            date = FindViewById<TextView>(Resource.Id.Details_Date);
            category = FindViewById<TextView>(Resource.Id.Details_BillCategory);
            desc = FindViewById<TextView>(Resource.Id.Details_BillDesc);
            image = FindViewById<ImageView>(Resource.Id.Details_imageView);
            updated = FindViewById<TextView>(Resource.Id.Details_Updated);
            editButton = FindViewById<Button>(Resource.Id.Details_EditButton);
            name_edit = FindViewById<EditText>(Resource.Id.Details_BillName_Edit);
            amount_edit = FindViewById<EditText>(Resource.Id.Details_BillAmount_Edit);
            date_edit = FindViewById<EditText>(Resource.Id.Details_Date_Edit);
            category_edit = FindViewById<Spinner>(Resource.Id.Details_BillCategory_Edit);
            desc_edit = FindViewById<EditText>(Resource.Id.Details_BillDesc_Edit);
            saveButton = FindViewById<Button>(Resource.Id.Details_SaveButton);

            // Populate the views
            name.Text = mBill.Name;
            amount.Text = "$" + String.Format("{0:0.00}", mBill.Amount); // rounds to 2 decimal places
            date.Text = mBill.Date.ToString("MMMM dd, yyyy");
            category.Text = mBill.Category;
            desc.Text = mBill.Description;

            // Initialize the Categories Spinner
            String[] categories = Resources.GetStringArray(Resource.Array.categories_array);
            int categoryIndex = 0;
            for (int i = 0; i < categories.Length; ++i)
            {
                if (categories[i] == category.Text)
                {
                    categoryIndex = i;
                    break;
                }
            }
            ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, categories);
            category_edit.Adapter = adapter;

			// Display in ImageView. We will resize the bitmap to fit the display.
			// Loading the full sized image will consume too much memory
			// and cause the application to crash.
			if (mBill.ImagePath != null)
			{
				int height = this.Resources.DisplayMetrics.HeightPixels;
				int width = this.Resources.DisplayMetrics.WidthPixels;
				Android.Graphics.Bitmap imageMap = mBill.ImagePath.LoadAndResizeBitmap(width, height);
				Android.Graphics.Bitmap emptyBitmap = Android.Graphics.Bitmap.CreateBitmap(imageMap.Width, imageMap.Height, imageMap.GetConfig());
				if (imageMap != null && !imageMap.SameAs(emptyBitmap))
				{
                    image.SetImageBitmap(imageMap);
                    image.Visibility = ViewStates.Visible;
                    // Dispose of the Java side bitmap.
                    GC.Collect();
                }
                else
                {
					image.Visibility = ViewStates.Gone;
                }
			}
			else
			{
				image.Visibility = ViewStates.Invisible;
			}
            updated.Text = "Last updated: " + mBill.LastEdited.ToString("MMMM dd, yyyy");

            // Instantiate ViewSwitchers used for switching to edit mode
            ViewSwitcher nameSwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_name_switcher);
            ViewSwitcher amountSwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_amount_switcher);
            ViewSwitcher categorySwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_category_switcher);
            ViewSwitcher descSwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_desc_switcher);
            ViewSwitcher buttonSwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_button_switcher);

            editButton.Click += delegate
                {
                    nameSwitcher.ShowNext();
                    amountSwitcher.ShowNext();
                    categorySwitcher.ShowNext();
                    descSwitcher.ShowNext();
                    buttonSwitcher.ShowNext();

                    name_edit.SetText(name.Text, TextView.BufferType.Editable);
                    amount_edit.SetText(amount.Text.Substring(1), TextView.BufferType.Editable); // dangerous string concat to get rid of dollar sign, error check later
                    date_edit.SetText(date.Text, TextView.BufferType.Editable);
                    category_edit.SetSelection(categoryIndex);
                    desc_edit.SetText(desc.Text, TextView.BufferType.Editable);
                };

            saveButton.Click += delegate
            {
                nameSwitcher.ShowPrevious();
                amountSwitcher.ShowPrevious();
                categorySwitcher.ShowPrevious();
                descSwitcher.ShowPrevious();
                buttonSwitcher.ShowPrevious();

                mBill.Name = name_edit.Text;
                mBill.Amount = Double.Parse(amount_edit.Text);
                mBill.Date = Convert.ToDateTime(date_edit.Text); // date format check here
                mBill.Category = adapter.GetItem(category_edit.SelectedItemPosition);
                mBill.Description = desc_edit.Text;

                mDBS.SaveBillEntry(mBill.Id, mBill);

                name.Text = name_edit.Text;
                amount.Text = "$" + amount_edit.Text; // number check here
                date.Text = date_edit.Text;
                category.Text = adapter.GetItem(category_edit.SelectedItemPosition);
                desc.Text = desc_edit.Text;

                Toast.MakeText(this, "Update Successful", ToastLength.Short).Show();
            };

        }

        //public override void OnBackPressed()
        //{
        //    base.OnBackPressed();
        //    FinishActivity(1);
        //}
        //protected override void OnStop()
        //{
        //    base.OnStop();
        //}
    }
}