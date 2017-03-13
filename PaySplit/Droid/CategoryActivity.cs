﻿using System;
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

	[Activity(Label = "Categories", MainLauncher = false, Icon = "@mipmap/new_icon")]
	public class CategoryActivity : Activity
	{
		private List<String> mCategories = new List<String>();
		private CategoryListViewAdapter mAdapter;

		private ListView mViewBillsListview;
		private ImageView mNoResultsImage;
		private TextView mNoResultsText;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main_ListView);

			mNoResultsText = FindViewById<TextView>(Resource.Id.NoResults);
			mNoResultsImage = FindViewById<ImageView>(Resource.Id.NoResultsImage);
			mViewBillsListview = FindViewById<ListView>(Resource.Id.View_ListView);

			// Setup adapter
			mAdapter = new CategoryListViewAdapter(this, mCategories);
			mViewBillsListview.Adapter = mAdapter;
		}

		protected override void OnResume()
		{
			base.OnResume();

			//Generate or Initialize Database Path
			DataHelper dbPath = new DataHelper();
			dbPath.CreateDataBase("PaySplitDataDb.db3");

			//Initialize database service
			GenDataService dbs = new GenDataService(dbPath.DBPath);

			//Create Table
			dbs.CreateTable();

			List<Bill> bills = dbs.GetAllBills();
			mCategories.Clear();
			foreach (Bill bill in bills)
			{
				if (!mCategories.Contains(bill.Category))
				{
					mCategories.Add(bill.Category);
				}
			}

			if (mCategories == null || mCategories.Count == 0)
			{
				mNoResultsText.Visibility = ViewStates.Visible;
				mNoResultsImage.Visibility = ViewStates.Visible;
			}

			mAdapter.update(mCategories);
			mViewBillsListview.Adapter = mAdapter;
		}
	}
}