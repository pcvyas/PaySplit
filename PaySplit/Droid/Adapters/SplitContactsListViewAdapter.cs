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
		public List<Boolean> mChecked = new List<bool>();
		public List<SplitContactListViewHolder> allContacts = new List<SplitContactListViewHolder>();
		private Context mContext;

		private bool mIsDialogShowing = false;

		public SplitContactsListViewAdapter(Context context, List<Contact> contacts)
		{
			System.Diagnostics.Debug.WriteLine("gets inside splitcontactslistviewadapter constructor");
			this.mContacts = contacts;
			for (int i = 0; i < this.mContacts.Count; i++)
			{
				this.mChecked.Add(false);
			}
			this.mContext = context;
			allContacts.Clear();
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

		public void update(List<Contact> contacts, List<Boolean> mChecked )
		{
			this.mContacts = contacts;
			this.mChecked = mChecked;
		}

		public List<SplitContactListViewHolder> getViewHolders()
		{
			return this.allContacts;
		}

		public List<Boolean> getMChecked()
		{
			return this.mChecked;
		}

		// Define what is within each row
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			System.Diagnostics.Debug.WriteLine("gets inside getView of splitcontactslistview");
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
			if (this.mChecked[position])
			{
				viewHolder.checkBox.Checked = true;
			}
			else
			{
				viewHolder.checkBox.Checked = false;
			}
			allContacts.Add(viewHolder);
			return rowView;
		}



		public class SplitContactListViewHolder : Java.Lang.Object
		{
			public TextView contactName;
			public TextView contactEmail;
			public CheckBox checkBox;

			public SplitContactListViewHolder(View view)
			{
				contactName = view.FindViewById<TextView>(Resource.Id.contactName);
				contactEmail = view.FindViewById<TextView>(Resource.Id.contactEmail);
				checkBox = view.FindViewById<CheckBox>(Resource.Id.checkBox);
			}
		}
	}
}