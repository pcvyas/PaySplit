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
	class ContactsListViewAdapter : BaseAdapter<Contact>
	{
		private List<Contact> mContacts;
		private Context mContext;

		private bool mIsDialogShowing = false;

		public ContactsListViewAdapter(Context context, List<Contact> contacts)
		{
			this.mContacts = contacts;
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

		// Define what is within each row
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View rowView = convertView;
			ContactListViewHolder viewHolder;

			// create a new row if not drawn
			if (rowView == null)
			{
				// We use a viewholder so the views do not have to be recreated
				rowView = LayoutInflater.From(mContext).Inflate(Resource.Layout.ViewContact, null, false);
				viewHolder = new ContactListViewHolder(rowView);
				rowView.Tag = viewHolder;
			}
			else
			{
				viewHolder = (ContactListViewHolder)rowView.Tag;
			}

			Contact c = mContacts[position];

			viewHolder.deleteButton.Click += delegate {
				if (!mIsDialogShowing)
				{
					showDeleteContactDialog(c);
				}
			};
			viewHolder.editButton.Click += delegate
			{
				if (!mIsDialogShowing)
				{
					showEditContactDialog(c);
				}
			};

			viewHolder.contactName.Text = c.FullName;
			viewHolder.contactEmail.Text = "Email: " + c.Email;

			return rowView;
		}

		private void showEditContactDialog(Contact c)
		{
			AlertDialog.Builder alertDialog = new AlertDialog.Builder(mContext);
			alertDialog.SetTitle("Edit Contact");
			TextView nameTextView = new TextView(mContext);
			nameTextView.Text = "Name:";
			EditText nameEditText = new EditText(mContext);
			nameEditText.SetSingleLine(true);
			nameEditText.Text = c.FullName;

			TextView emailTextView = new TextView(mContext);
			emailTextView.Text = "E-mail:";
			EditText emailEditText = new EditText(mContext);
			emailEditText.SetSingleLine(true);
			emailEditText.Text = c.Email;

			LinearLayout ll = new LinearLayout(mContext);
			ll.Orientation = Orientation.Vertical;
			ll.AddView(nameTextView);
			ll.AddView(nameEditText);
			ll.AddView(emailTextView);
			ll.AddView(emailEditText);
			ll.SetPadding(25, 25, 25, 25);
			alertDialog.SetView(ll);

			alertDialog.SetCancelable(false);
			alertDialog.SetPositiveButton("Update", delegate {
				string name = nameEditText.Text;
				string email = emailEditText.Text;

				Contact contact = c;
				contact.FullName = name;
				contact.Email = email;
				DataHelper.getInstance().getGenDataService().UpdateContactInformation(contact);
			});
			alertDialog.SetNegativeButton("Cancel", delegate {
				mIsDialogShowing = false;
			});

			AlertDialog dialog = alertDialog.Create();
			dialog.Show();

			mIsDialogShowing = true;
		}

		private void showDeleteContactDialog(Contact c)
		{
			AlertDialog.Builder alertDialog = new AlertDialog.Builder(mContext);
			alertDialog.SetTitle("Delete Contact");
			alertDialog.SetMessage("Are you sure you want to delete " + c.FullName + " from your contacts? This will also remove them from any bills you have created.");
			alertDialog.SetCancelable(false);
			alertDialog.SetPositiveButton("Confirm", delegate
			{
				DataHelper.getInstance().getGenDataService().DeleteContact(c);
			});
			alertDialog.SetNegativeButton("Cancel", delegate
			{
				mIsDialogShowing = false;
			});

			AlertDialog dialog = alertDialog.Create();
			dialog.Show();
			mIsDialogShowing = true;
		}
	}
	public class ContactListViewHolder : Java.Lang.Object
	{
		public TextView contactName;
		public TextView contactEmail;
		public Button editButton;
		public Button deleteButton;

		public ContactListViewHolder(View view)
		{
			contactName = view.FindViewById<TextView>(Resource.Id.contactName);
			contactEmail = view.FindViewById<TextView>(Resource.Id.contactEmail);
			editButton = view.FindViewById<Button>(Resource.Id.Contact_EditBtn);
			deleteButton = view.FindViewById<Button>(Resource.Id.Contact_DelteBtn);
		}
	}
}