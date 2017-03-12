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
            ViewBillListViewHolder viewHolder;

            // create a new row if not drawn
            if (rowView == null)
            {
                // We use a viewholder so the views do not have to be recreated
                rowView = LayoutInflater.From(context).Inflate(Resource.Layout.ViewBills, null, false);
                viewHolder = new ViewBillListViewHolder(rowView);
                rowView.Tag = viewHolder;
            } else
            {
                viewHolder = (ViewBillListViewHolder)rowView.Tag;
            }

            // Define what is in the row
            // Assign the text field of the textview to the name of each bill
            viewHolder.billName.Text = bills[position].Name;

            viewHolder.billDesc.Text = "Last modified: " + bills[position].LastEdited;

            viewHolder.billCategory.Text = bills[position].Category;

			rowView.Click += delegate
            {
                var activity = new Intent(context, typeof(BillDetailsActivity));
                activity.PutExtra("id", bills[position].Id.ToString());
                context.StartActivity(activity);
                //((Activity)context).StartActivityForResult(activity, 1);
             };

            return rowView;
        }

        // Update the bills list
        public void update(List<Bill> bills)
        {
            this.bills = bills;
        }
    }

    public class ViewBillListViewHolder : Java.Lang.Object
    {
        public TextView billName;
        public TextView billDesc;
        public TextView billCategory;
        public ViewBillListViewHolder(View view)
        {
            billName = view.FindViewById<TextView>(Resource.Id.billTitle);
            billDesc = view.FindViewById<TextView>(Resource.Id.billDescription);
            billCategory = view.FindViewById<TextView>(Resource.Id.billCategory);
        }
    }
}