using Android.App;
using Android.OS;
using Android.Content;

using Android.Net;
using System;
using Java.IO;
using Java.Lang;
using Android.Widget;

namespace PaySplit.Droid
{
	[Activity(Label = "PaySplit", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
	[IntentFilter(new string[] { Intent.ActionView },
		Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
		DataScheme = "content",
		DataHost = "*",
		DataMimeType = "application/psbf"
	)]
	[IntentFilter(new string[] { Intent.ActionView },
		Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
		DataScheme = "content",
		DataHost = "*",
		DataMimeType = "application/octet-stream"
	)]
	public class MainActivity : Activity
	{

		private DataHelper mDataHelper;
		private GenDataService mDBS;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.CreateUser);

			mDataHelper = DataHelper.getInstance();
			mDataHelper.getGenDataService().CreateTableIfNotExists();

			mDBS = DataHelper.getInstance().getGenDataService();

			bool user_info_recorded = Settings.getUserCreated(this);

			Android.Net.Uri data = Intent.Data;
			if (data != null && user_info_recorded)
			{
				// reset the data
				Intent.SetData(null);
				try
				{
					extractBillData(data);
				}
				catch (System.Exception)
				{
					Toast.MakeText(this, "Error: Failed to load bill from e-mail.", ToastLength.Short).Show(); 
				}
			}

			if (!user_info_recorded)
			{
				// send user to create user page (so we can record their name, email, etc) on first use and
				// persist this data for later
				StartActivity(typeof(CreateUserActivity));
				Finish();
			}
			else
			{
				// The user has already registered, so we can just send them to the main view Bills Activity
				StartActivity(typeof(ViewBillsActivity));
				Finish();
			}
		}

		private void extractBillData(Android.Net.Uri data)
		{
			string scheme = data.Scheme;
			if (ContentResolver.SchemeContent.Equals(scheme))
			{
				System.IO.Stream stream = ContentResolver.OpenInputStream(data);
				if (stream == null) return;
				BufferedReader reader = new BufferedReader(new InputStreamReader(stream));
				if (stream != null)
				{
					/* Load Contacts */
					int numContacts = Integer.ParseInt(reader.ReadLine());
					for (int i = 0; i < numContacts; i++)
					{
						string name = reader.ReadLine();
						string email = reader.ReadLine();
						Contact c = new Contact(name, email);
						mDBS.InsertContactEntry(c);
					}

					reader.ReadLine();

					/* Load Bill */
					string billUid = reader.ReadLine();
					string title = reader.ReadLine();
					string description = reader.ReadLine();
					string category = reader.ReadLine();
					double amount = Java.Lang.Double.ParseDouble(reader.ReadLine());
					DateTime date = DateTime.Parse(reader.ReadLine());
					DateTime lastEdit = DateTime.Parse(reader.ReadLine());
					string ownerEmail = reader.ReadLine();
					Bill b = new Bill(billUid);
					b.Name = title;
					b.Description = description;
					b.Category = category;
					b.Amount = amount;
					b.Date = date;
					b.LastEdited = lastEdit;
					b.OwnerEmail = ownerEmail;
					mDBS.InsertBillEntry(b);

					reader.ReadLine();

					/* Load Transactions */
					int numTransactions = Integer.ParseInt(reader.ReadLine());
					for (int i = 0; i < numTransactions; i++)
					{
						Transaction t = new Transaction(reader.ReadLine());
						t.BillUID = reader.ReadLine();
						t.SenderEmail = reader.ReadLine();
						t.ReceiverEmail = reader.ReadLine();
						t.Amount = Java.Lang.Double.ParseDouble(reader.ReadLine());
						mDBS.InsertTransactionEntry(t);
					}
				}
				stream.Close();
			}
		}
	}
}