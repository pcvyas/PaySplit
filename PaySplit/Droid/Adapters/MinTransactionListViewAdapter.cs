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
    class MinTransactionListViewAdapter : BaseAdapter<Transaction>
    {
        private List<Transaction> mTransactions;
        private Context mContext;

        public MinTransactionListViewAdapter(Context context, List<Transaction> transactions)
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

        public override Transaction this[int position]
        {
            get { return mTransactions[position]; }
        }

        public void update(List<Transaction> transactions)
        {
            this.mTransactions = transactions;
            NotifyDataSetChanged();
            NotifyDataSetInvalidated();
        }

        // Define what is within each row
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View rowView = convertView;
            TransactionListViewHolder viewHolder;

            // create a new row if not drawn
            if (rowView == null)
            {
                // We use a viewholder so the views do not have to be recreated
                rowView = LayoutInflater.From(mContext).Inflate(Resource.Layout.ViewMinBalances, null, false);
                viewHolder = new TransactionListViewHolder(rowView);
                rowView.Tag = viewHolder;
            }
            else
            {
                viewHolder = (TransactionListViewHolder)rowView.Tag;
            }

            Transaction t = mTransactions[position];

            viewHolder.amount.Text = t.Amount.ToString();
            viewHolder.payer.Text = t.SenderEmail;
            viewHolder.payee.Text = t.ReceiverEmail;

            return rowView;
        }

    }
	public class TransactionListViewHolder : Java.Lang.Object
	{
		public TextView amount;
		public TextView payer;
        public TextView payee;

		public TransactionListViewHolder(View view)
		{
            amount = view.FindViewById<TextView>(Resource.Id.amount_text);
            payer = view.FindViewById<TextView>(Resource.Id.min_payer);
            payee = view.FindViewById<TextView>(Resource.Id.min_payee);
		}
	}
}