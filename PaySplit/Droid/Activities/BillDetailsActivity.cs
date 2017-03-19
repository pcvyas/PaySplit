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
    [Activity(Label = "Bill Details", MainLauncher = false, Icon = "@mipmap/ic_launcher")]
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
		private Button deleteButton;

		private EditText name_edit;
		private EditText amount_edit;
		private TextView date_edit;
		private Spinner category_edit;
		private EditText desc_edit;
		private Button saveButton;

		// Instantiate ViewSwitchers used for switching to edit mode
		private ViewSwitcher nameSwitcher;
		private ViewSwitcher amountSwitcher;
		private ViewSwitcher categorySwitcher;
		private ViewSwitcher descSwitcher;
		private ViewSwitcher buttonSwitcher;
		private ViewSwitcher dateSwitcher;

		ArrayAdapter<String> adapter;

		int categoryIndex = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BillDetails);

            //string id = Intent.GetStringExtra("id") ?? "Data not available";
            string id = Intent.GetStringExtra("id");

			//Initialize database service
			mDBS = DataHelper.getInstance().getGenDataService();
            mBill = mDBS.getBillById(Int32.Parse(id));
			if (mBill == null)
			{
				Finish();
				return;
			}

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
			date_edit = FindViewById<TextView>(Resource.Id.Details_Date_Edit);
            category_edit = FindViewById<Spinner>(Resource.Id.Details_BillCategory_Edit);
            desc_edit = FindViewById<EditText>(Resource.Id.Details_BillDesc_Edit);
            saveButton = FindViewById<Button>(Resource.Id.Details_SaveButton);
			deleteButton = FindViewById<Button>(Resource.Id.deleteBillButton);

			// Instantiate ViewSwitchers used for switching to edit mode
			nameSwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_name_switcher);
			amountSwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_amount_switcher);
			categorySwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_category_switcher);
			descSwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_desc_switcher);
			buttonSwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_button_switcher);
			dateSwitcher = FindViewById<ViewSwitcher>(Resource.Id.Details_date_switcher);

			// Populate the views
			name.Text = mBill.Name;
			amount.Text = "$" + String.Format("{0:0.00}", mBill.Amount); // rounds to 2 decimal places
			date.Text = mBill.Date.ToString("MMMM dd, yyyy");
			category.Text = mBill.Category;
			desc.Text = mBill.Description;
			    
            // Initialize the Categories Spinner
            String[] categories = Resources.GetStringArray(Resource.Array.categories_array);
            categoryIndex = 0;
            for (int i = 0; i < categories.Length; ++i)
            {
                if (categories[i] == category.Text)
                {
                    categoryIndex = i;
                    break;
                }
            }
            adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, categories);
            category_edit.Adapter = adapter;

			LoadImageForBill();
			updateLastUpdatedTimestampText();

			editButton.Click += Edit_Click;
			saveButton.Click += Save_Click;
			deleteButton.Click += Delete_Click;

			this.ActionBar.SetDisplayHomeAsUpEnabled(true);
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

		private void updateLastUpdatedTimestampText()
		{
			updated.Text = "Last updated: " + mBill.LastEdited.ToString("MMMM dd, yyyy");
		}

		void LoadImageForBill()
		{
			// Display in ImageView. We will resize the bitmap to fit the display.
			// Loading the full sized image will consume too much memory
			// and cause the application to crash.
			if (mBill.ImagePath != null)
			{
				int height = this.Resources.DisplayMetrics.HeightPixels;
				int width = this.Resources.DisplayMetrics.WidthPixels;
				Android.Graphics.Bitmap imageMap = mBill.ImagePath.LoadAndResizeBitmap(width, height);
				//Android.Graphics.Bitmap emptyBitmap = Android.Graphics.Bitmap.CreateBitmap(imageMap.Width, imageMap.Height, imageMap.GetConfig());
				if (imageMap != null)
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
				image.Visibility = ViewStates.Gone;
			}
		}

		void Edit_Click(object sender, EventArgs e)
		{
			nameSwitcher.ShowNext();
			amountSwitcher.ShowNext();
			categorySwitcher.ShowNext();
			descSwitcher.ShowNext();
			buttonSwitcher.ShowNext();
			dateSwitcher.ShowNext();

			date_edit.Click += Date_Click;
			date_edit.Background = GetDrawable(Resource.Drawable.big_card);

			name_edit.SetText(name.Text, TextView.BufferType.Editable);
			amount_edit.SetText(amount.Text.Substring(1), TextView.BufferType.Editable); // dangerous string concat to get rid of dollar sign, error check later
			date_edit.SetText(date.Text, TextView.BufferType.Editable);
			category_edit.SetSelection(categoryIndex);
			desc_edit.SetText(desc.Text, TextView.BufferType.Editable);
		}

		void Save_Click(object sender, EventArgs e)
		{
			nameSwitcher.ShowPrevious();
			amountSwitcher.ShowPrevious();
			categorySwitcher.ShowPrevious();
			descSwitcher.ShowPrevious();
			buttonSwitcher.ShowPrevious();
			dateSwitcher.ShowPrevious();

			date_edit.Click -= Date_Click;
			date_edit.Background = null;

			mBill.Name = name_edit.Text;
			mBill.Amount = Double.Parse(amount_edit.Text);
			mBill.Date = Convert.ToDateTime(date_edit.Text); // date format check here
			mBill.Category = adapter.GetItem(category_edit.SelectedItemPosition);
			mBill.Description = desc_edit.Text;
			mBill.LastEdited = DateTime.Now;

			mDBS.SaveBillEntry(mBill.Id, mBill);

			name.Text = name_edit.Text;
			amount.Text = "$" + amount_edit.Text; // number check here
			date.Text = date_edit.Text;
			category.Text = adapter.GetItem(category_edit.SelectedItemPosition);
			desc.Text = desc_edit.Text;
		}

		void Delete_Click(object sender, EventArgs e)
		{
			mDBS.DeleteBillAsync(mBill);
			this.Finish();
		}

		void Date_Click(object sender, EventArgs e)
		{
			//TextView dateV = FindViewById<TextView>(Resource.Id.Details_Date);
			DatePickerFragment frag = 
				DatePickerFragment.NewInstance(delegate (DateTime date)
												 {
													 //dateV.Text = date.ToLongDateString();
													 date_edit.Text = date.ToLongDateString();
													 mBill.Date = date;
												 });
			frag.Show(FragmentManager, DatePickerFragment.TAG);
		}
    }
}