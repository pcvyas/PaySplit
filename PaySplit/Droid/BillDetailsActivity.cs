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
    [Activity(Label = "BillDetailsActivity")]
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

            // Instatiate text views
            TextView name = FindViewById<TextView>(Resource.Id.Details_BillName);
            TextView amount = FindViewById<TextView>(Resource.Id.Details_BillAmount);
            TextView owner = FindViewById<TextView>(Resource.Id.Details_Owner);
            TextView created = FindViewById<TextView>(Resource.Id.Details_CreatedOn);
            TextView updated = FindViewById<TextView>(Resource.Id.Details_Updated);
            TextView desc = FindViewById<TextView>(Resource.Id.Details_BillDesc);
            name.Text = bill.Name;
            amount.Text = bill.Amount.ToString();
            owner.Text = bill.Owner;
            created.Text = bill.Date.ToString();
            updated.Text = bill.LastEdited.ToString();
            desc.Text = bill.Description;
        }
    }
}