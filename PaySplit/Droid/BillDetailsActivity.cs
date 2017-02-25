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
        private Bill bill;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BillDetails);

            //string id = Intent.GetStringExtra("id") ?? "Data not available";
            string id = Intent.GetStringExtra("id");

            //Generate or Initialize Database Path
            DataHelper dbPath = new DataHelper();
            dbPath.CreateDataBase("PaySplitDataDb.db3");

            //Initialize database service
            GenDataService dbs = new GenDataService(dbPath.DBPath);

            //Create Table
            dbs.CreateTable();

            bill = dbs.getBillById(Int32.Parse(id));

            // Instatiate views
            TextView name = FindViewById<TextView>(Resource.Id.Details_BillName);
            EditText edit_name = FindViewById<EditText>(Resource.Id.Details_BillName_Edit);
            TextView amount = FindViewById<TextView>(Resource.Id.Details_BillAmount);
            TextView date = FindViewById<TextView>(Resource.Id.Details_Date);
            TextView category = FindViewById<TextView>(Resource.Id.Details_BillCategory);
            EditText desc = FindViewById<EditText>(Resource.Id.Details_BillDesc);
            ImageView image = FindViewById<ImageView>(Resource.Id.Details_imageView);
            TextView updated = FindViewById<TextView>(Resource.Id.Details_Updated);
            Button editButton = FindViewById<Button>(Resource.Id.Details_EditButton);

            name.Text = bill.Name;
            amount.Text = "$" + (bill.Amount == Math.Round(bill.Amount) ? bill.Amount + ".00" : bill.Amount.ToString());
            date.Text = "Date: " + bill.Date.ToString("MMMM dd, yyyy");
            category.Text = bill.Category;
            desc.Text = bill.Description;
            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume too much memory
            // and cause the application to crash.
            int height = this.Resources.DisplayMetrics.HeightPixels;
            int width = this.Resources.DisplayMetrics.WidthPixels;
            Android.Graphics.Bitmap imageMap = bill.ImagePath.LoadAndResizeBitmap(width, height);
            if (App.bitmap != null)
            {
                image.SetImageBitmap(App.bitmap);
                App.bitmap = null;

                image.Visibility = ViewStates.Visible;
                // Dispose of the Java side bitmap.
                GC.Collect();
            }
            else
            {
                image.Visibility = ViewStates.Invisible;
                App.file = null;
            }
            updated.Text = "Last updated: " + "Date: " + bill.LastEdited.ToString("MMMM dd, yyyy");

            editButton.Click += delegate
                {
                    name.Visibility = ViewStates.Invisible;
                    edit_name.Visibility = ViewStates.Visible;
                };
        }
    }
}