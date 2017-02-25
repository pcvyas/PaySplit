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
    class BillListViewAdapter : BaseAdapter<string>
    {
        private List<Bill> bills;
        private Context context;

        public BillListViewAdapter(Context context, List<Bill> bills)
        {
            this.bills = bills;
            this.context = context;
        }
        public override int Count
        {
            get { return bills.Count; }
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        // indexer that makes the adapter array-like
        public override string this[int position]
        {
            get { return bills[position].Name; }
        }

        // Define what is within each row
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View rowView = convertView;

            // create a new row if not drawn
            if (rowView == null)
            {
                rowView = LayoutInflater.From(context).Inflate(Resource.Layout.ViewBills, null, false);
            }

            // Define what is in the row
            // Assign the text field of the textview to the name of each bill
			TextView billName = rowView.FindViewById<TextView>(Resource.Id.billTitle);
            billName.Text = bills[position].Name;

			TextView billDesc = rowView.FindViewById<TextView>(Resource.Id.billDescription);
			billDesc.Text = "Last modified: " + bills[position].LastEdited;

			TextView billCategory = rowView.FindViewById<TextView>(Resource.Id.billCategory);
			billCategory.Text = bills[position].Category;

			rowView.Click += delegate
            {
                var activity = new Intent(context, typeof(BillDetailsActivity));
                activity.PutExtra("id", bills[position].Id.ToString());
                context.StartActivity(activity);
            };

            return rowView;
        }
    }
}