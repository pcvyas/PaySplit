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
using Java.IO;
using Java.Lang;

namespace PaySplit.Droid
{
    class TransactionListViewAdapter : BaseAdapter<string>
    {
		private List<Transaction> mTransactions;
		private Context mContext;

		public TransactionListViewAdapter(Context context, List<Transaction> transactions)
        {
			this.mTransactions = transactions;
            this.mContext = context;
        }
        public override int Count
        {
			get { return mTransactions.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        // indexer that makes the adapter array-like
        public override string this[int position]
        {
			get { return mTransactions[position].ReceiverEmail; }
        }

        // Define what is within each row
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View rowView = convertView;
            TransactionsListViewHolder viewHolder;

            // create a new row if not drawn
            if (rowView == null)
            {
                // We use a viewholder so the views do not have to be recreated
				rowView = LayoutInflater.From(mContext).Inflate(Resource.Layout.TransactionView, null, false);
                viewHolder = new TransactionsListViewHolder(rowView);
                rowView.Tag = viewHolder;
            }
			else
            {
                viewHolder = (TransactionsListViewHolder)rowView.Tag;
            }

			Transaction trans = mTransactions[position];

            // Define what is in the row
            // Assign the text field of the textview to the name of each bill
			viewHolder.sender.Text = mTransactions[position].SenderEmail;
			viewHolder.reciever.Text = mTransactions[position].ReceiverEmail;
			viewHolder.amount.Text = "$" + mTransactions[position].Amount.ToString();

			var dbs = DataHelper.getInstance().getGenDataService();

			viewHolder.sendEmail.Click += delegate
			{
				File outputDir = mContext.GetExternalCacheDirs().FirstOrDefault();
				File outputFile = File.CreateTempFile("BillShare", ".psbf", outputDir);
				Android.Net.Uri path = Android.Net.Uri.FromFile(outputFile);

				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				//------------Create PBSF file Here
				List<Transaction> transactionsWithBill = dbs.getTransactionsForBill(trans.BillUID);

				//Contacts Properties
				var contacts = transactionsWithBill.Select(o => o.SenderEmail).ToList();
				sb.AppendLine(contacts.Count().ToString());
				for (int i = 0; i < contacts.Count(); i++)
				{
					Contact c = dbs.getContactByEmail(contacts[i]);
					if (c.Email != trans.ReceiverEmail)
					{
						sb.AppendLine(c.FullName);
						sb.AppendLine(c.Email);
					}
				}

				//Bills detail
				sb.AppendLine("");
				Bill b = dbs.getBillByUID(trans.BillUID);
				sb.AppendLine(b.UID);
				sb.AppendLine(b.Name);
				sb.AppendLine(b.Description);
				sb.AppendLine(b.Category);
				sb.AppendLine(b.Amount.ToString());
				sb.AppendLine(b.Date.ToString());
				sb.AppendLine(b.LastEdited.ToString());
				sb.AppendLine(b.OwnerEmail);

				//Trans Detail
				sb.AppendLine("");
				sb.AppendLine(transactionsWithBill.Count.ToString());
				for (int i = 0; i < transactionsWithBill.Count; i++)
				{
					sb.AppendLine(transactionsWithBill[i].UID);
					sb.AppendLine(transactionsWithBill[i].BillUID);
					sb.AppendLine(transactionsWithBill[i].SenderEmail);
					sb.AppendLine(transactionsWithBill[i].ReceiverEmail);
					sb.AppendLine(Java.Lang.Double.ToString(transactionsWithBill[i].Amount));
				}

				string filetoWrite = sb.ToString();
				var writer = new BufferedWriter(new FileWriter(outputFile));
				writer.Write(filetoWrite);

				writer.Flush();
				writer.Close();

                //--------------------------------------
				Intent emailIntent = new Intent(Intent.ActionSend);

				// set the type to 'email'
				emailIntent.SetType("vnd.android.cursor.dir/email");

				//To address
				string[] to = { trans.SenderEmail };
				emailIntent.PutExtra(Intent.ExtraEmail, to);

				// the attachment
				emailIntent.PutExtra(Intent.ExtraStream, path);

				// the mail subject

				emailIntent.PutExtra(Intent.ExtraEmail, new string[] { mTransactions[position].SenderEmail });
				emailIntent.PutExtra(Intent.ExtraSubject, "PaySplit Bill file");

				Java.Lang.StringBuilder builder = new Java.Lang.StringBuilder();
				builder.Append("Hello,\nYou are receiving this email because " + dbs.getContactByEmail(b.OwnerEmail).FullName
									+ " has decided to share a bill for " + b.Name + " with you.\n\n");

				builder.Append("You owe a total of $" + trans.Amount + "! You can import this bill into your PaySplit app by clicking the attached pay split bill file, or simply use a payment service of your choice to pay this user.\n\n");
				builder.Append("Thanks,\nPaySplit Team");
				emailIntent.PutExtra(Intent.ExtraText, builder.ToString());
				mContext.StartActivity(Intent.CreateChooser(emailIntent, "Choose email client to send with..."));

			};
			
             return rowView;
        }

        // Update the bills list
		public void update(List<Transaction> transactions)
        {
			this.mTransactions = transactions;
        }

		public List<Transaction> getTransactions()
        {
			return this.mTransactions;
        }

    }

    public class TransactionsListViewHolder : Java.Lang.Object
    {
        public TextView reciever;
        public TextView sender;
        public TextView amount;
		public ImageButton sendEmail;
        public TransactionsListViewHolder(View view)
        {
			reciever = view.FindViewById<TextView>(Resource.Id.reciever);
			sender = view.FindViewById<TextView>(Resource.Id.sender);
			amount = view.FindViewById<TextView>(Resource.Id.transAmount);
			sendEmail = view.FindViewById<ImageButton>(Resource.Id.sendEmail);
        }
    }
}