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

		// indexer that makes the adapter array-like
		public override string this[int position]
		{
			get { return categories[position]; }
		}

		// Define what is within each row
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View rowView = convertView;

			// create a new row if not drawn
			if (rowView == null)
			{
				rowView = LayoutInflater.From(context).Inflate(Resource.Layout.ViewCategory, null, false);
			}

			// Define what is in the row
			// Assign the text field of the textview to the name of each bill
			TextView categoryItem = rowView.FindViewById<TextView>(Resource.Id.CategoryName);
			categoryItem.Text = categories[position];
			categoryItem.Click += delegate
			{
				var activity = new Intent(context, typeof(ViewBillsActivity));
				activity.PutExtra("category", categories[position]);
				context.StartActivity(activity);
			};

			return rowView;
		}
	}
}