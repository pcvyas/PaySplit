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
	class SplitContactsListViewAdapter : BaseAdapter<Contact>
	{
		private List<Contact> mContacts = new List<Contact>();
		private List<Boolean> mChecked = new List<bool>();
		private string mOwnerEmail;
		private Context mContext;

		private bool mIsDialogShowing = false;

		public SplitContactsListViewAdapter(Context context, List<Contact> contacts, string ownerEmail)
		{
			this.mContacts = contacts;
			for (int i = 0; i < this.mContacts.Count; i++)
			{
				this.mChecked.Add(mContacts[i].Email.Equals(ownerEmail));
			}
			this.mOwnerEmail = ownerEmail;
			this.mContext = context;
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

		public void update(List<Contact> contacts)
		{
			this.mContacts = contacts;
		}

		public List<Contact> getSelectedContacts()
		{
			int i = 0;
			List<Contact> result = new List<Contact>();
			foreach (bool chk in mChecked)
			{
				if (mChecked[i])
				{
					result.Add(mContacts[i]);
				}
				i++;
			}
			return result;
		}

		// Define what is within each row
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View rowView = convertView;
			SplitContactListViewHolder viewHolder;

			// create a new row if not drawn
			if (rowView == null)
			{
				// We use a viewholder so the views do not have to be recreated
				rowView = LayoutInflater.From(mContext).Inflate(Resource.Layout.SplitView, null, false);
				viewHolder = new SplitContactListViewHolder(rowView);
				rowView.Tag = viewHolder;
			}
			else
			{
				viewHolder = (SplitContactListViewHolder)rowView.Tag;
			}

			Contact c = mContacts[position];
			viewHolder.contactName.Text = c.FullName;
			viewHolder.contactEmail.Text = "Email: " + c.Email;
			viewHolder.checkBox.Checked = mChecked[position];
			if (c.Email.Equals(mOwnerEmail))
			{
				viewHolder.checkBox.Enabled = false;
			}
			viewHolder.checkBox.CheckedChange += delegate {
				mChecked[position] = viewHolder.checkBox.Checked;
			};

			return rowView;
		}


	}
}