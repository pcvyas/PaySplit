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
	class CategoryListViewAdapter : BaseAdapter<string>
	{
		private List<String> categories;
		private Context context;

		public CategoryListViewAdapter(Context context, List<String> categories)
		{
			this.categories = categories;
			this.context = context;
		}

		public override int Count
		{
			get { return categories.Count; }
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override string this[int position]
		{
			get { return categories[position]; }
		}

		public void update(List<String> categories)
		{
			this.categories = categories;
		}

		// Define what is within each row
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View rowView = convertView;
            CategoryListViewHolder viewHolder;

            // create a new row if not drawn
            if (rowView == null)
			{
                // We use a viewholder so the views do not have to be recreated
                rowView = LayoutInflater.From(context).Inflate(Resource.Layout.ViewCategory, null, false);
                viewHolder = new CategoryListViewHolder(rowView);
                rowView.Tag = viewHolder;
            } else
            {
                viewHolder = (CategoryListViewHolder)rowView.Tag;
            }

            viewHolder.categoryItem.Text = categories[position];
            viewHolder.categoryItem.Click += delegate
			{
				var activity = new Intent(context, typeof(ViewBillsActivity));
				activity.PutExtra("category", categories[position]);
				context.StartActivity(activity);
			};

			return rowView;
		}
	}
    public class CategoryListViewHolder : Java.Lang.Object
    {
        public TextView categoryItem;
        public CategoryListViewHolder(View view)
        {
            categoryItem = view.FindViewById<TextView>(Resource.Id.CategoryName);
        }
    }
}