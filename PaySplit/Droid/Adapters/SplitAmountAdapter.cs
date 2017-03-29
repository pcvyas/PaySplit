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
	class SplitAmountAdapter : BaseAdapter<Contact>
	{
		public List<Contact> mContacts = new List<Contact>();
		public List<double> mAmounts = new List<double>();
		private Context mContext;
		private double total;

		public SplitAmountAdapter(Context context, List<Contact> contacts, double total)
		{
			this.mContacts = contacts;
			this.mContext = context;
			this.total = total;
			InitializeAmounts();
		}

		public override int Count
		{
			get { return mContacts.Count; }
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override Contact this[int position]
		{
			get { return mContacts[position]; }
		}

		public void setTotal(double total)
		{
			this.total = total;
			InitializeAmounts();
			NotifyDataSetChanged();
		}

		public void update(List<Contact> contacts)
		{
			this.mContacts = contacts;
			InitializeAmounts();
			NotifyDataSetChanged();
		}

		void InitializeAmounts()
		{
			mAmounts.Clear();
			for (int i = 0; i < mContacts.Count; i++)
			{
				mAmounts.Add(this.total / (this.mContacts.Count));
			}
		}

		public double getCoveredAmount()
		{
			double total = 0;
			foreach (double amount in mAmounts)
			{
				total += amount;
			}
			return total;
		}

		// Define what is within each row
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View rowView = convertView;
			SplitAmountViewHolder viewHolder;

			// create a new row if not drawn
			if (rowView == null)
			{
				// We use a viewholder so the views do not have to be recreated
				rowView = LayoutInflater.From(mContext).Inflate(Resource.Layout.SplitAmountView, null, false);
				viewHolder = new SplitAmountViewHolder(rowView);
				rowView.Tag = viewHolder;
			}
			else
			{
				viewHolder = (SplitAmountViewHolder)rowView.Tag;
			}

			Contact c = mContacts[position];
			viewHolder.contactName.Text = c.FullName;
			viewHolder.contactEmail.Text = "Email: " + c.Email;
			viewHolder.amount.Text = mAmounts[position].ToString("#.##");
			viewHolder.amount.TextChanged += delegate {
				try
				{
					mAmounts[position] = Double.Parse(viewHolder.amount.Text);
				}
				catch (Exception)
				{
					// failed to update amounts
				}
			};

			return rowView;
		}

		public class SplitAmountViewHolder : Java.Lang.Object
		{
			public TextView contactName;
			public TextView contactEmail;
			public EditText amount;

			public SplitAmountViewHolder(View view)
			{
				contactName = view.FindViewById<TextView>(Resource.Id.contactName);
				contactEmail = view.FindViewById<TextView>(Resource.Id.contactEmail);
				amount = view.FindViewById<EditText>(Resource.Id.splitAmount);
			}
		}
	}
}