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
using Java.Lang;

namespace PaySplit.Droid
{
    class BillListViewAdapter : BaseAdapter<string>
    {
		private List<Bill> mBills;
		private Context mContext;

        public BillListViewAdapter(Context context, List<Bill> bills)
        {
            this.mBills = bills;
            this.mContext = context;
        }
        public override int Count
        {
            get { return mBills.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        // indexer that makes the adapter array-like
        public override string this[int position]
        {
            get { return mBills[position].Name; }
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
                rowView = LayoutInflater.From(mContext).Inflate(Resource.Layout.ViewBills, null, false);
                viewHolder = new ViewBillListViewHolder(rowView);
                rowView.Tag = viewHolder;
            } else
            {
                viewHolder = (ViewBillListViewHolder)rowView.Tag;
            }

			Bill bill = mBills[position];

            // Define what is in the row
            // Assign the text field of the textview to the name of each bill
            viewHolder.billName.Text = mBills[position].Name;

			DateTime date = bill.Date;

			viewHolder.billDate.Text = date.ToString("MMM").ToUpper() + "\n" + date.Day;
            viewHolder.billDesc.Text = "Last modified: " + bill.LastEdited;
            viewHolder.billCategory.Text = bill.Category;
			rowView.Click += delegate
            {
                var activity = new Intent(mContext, typeof(BillDetailsActivity));
				activity.PutExtra("uid", bill.UID);
                mContext.StartActivity(activity);
             };

            return rowView;
        }

        // Update the bills list
        public void update(List<Bill> bills)
        {
            this.mBills = bills;
        }

        public List<Bill> getBills()
        {
            return this.mBills;
        }

    }

    public class ViewBillListViewHolder : Java.Lang.Object
    {
        public TextView billName;
        public TextView billDesc;
        public TextView billCategory;
		public TextView billDate;
        public ViewBillListViewHolder(View view)
        {
            billName = view.FindViewById<TextView>(Resource.Id.billTitle);
            billDesc = view.FindViewById<TextView>(Resource.Id.billDescription);
            billCategory = view.FindViewById<TextView>(Resource.Id.billCategory);
			billDate = view.FindViewById<TextView>(Resource.Id.date_text);
        }
    }
}