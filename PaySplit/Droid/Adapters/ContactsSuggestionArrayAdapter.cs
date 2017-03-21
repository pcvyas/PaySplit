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
	class ContactsSuggestionArrayAdapter : BaseAdapter<Contact>
	{
		private List<Contact> mContacts;
		private Context context;

		public ContactsSuggestionArrayAdapter(Context context, List<Contact> contacts)
		{
			this.mContacts = contacts;
			this.context = context;
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

		// Define what is within each row
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View rowView = convertView;
			ContactSuggestionListViewHolder viewHolder;
			if (rowView == null)
			{
				rowView = LayoutInflater.From(context).Inflate(Resource.Layout.SpinnerSuggestionView, null, false);
				viewHolder = new ContactSuggestionListViewHolder(rowView);
				rowView.Tag = viewHolder;
			}
			else
			{
				viewHolder = (ContactSuggestionListViewHolder)rowView.Tag;
			}

			Contact contact = mContacts[position];
			viewHolder.name.Text = contact.FullName;
			return rowView;
		}
	}
	public class ContactSuggestionListViewHolder : Java.Lang.Object
	{
		public TextView name;
		public ContactSuggestionListViewHolder(View view)
		{
			name = view.FindViewById<TextView>(Resource.Id.ownerName);
		}
	}
}