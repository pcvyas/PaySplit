using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;

namespace PaySplit.Droid
{
	[Activity(Label = "PaySplit", MainLauncher = true, Icon = "@mipmap/new_icon")]
	public class MainActivity : Activity
	{
		int count = 1;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.CreateEntry);


			//Generate or Initialize Database Path
			DataHelper dbPath = new DataHelper();
			dbPath.CreateDataBase("PaySplitDataDb.db3");

			//Initialize database service
			GenDataService dbs = new GenDataService(dbPath.DBPath);

			//Create Table
			dbs.CreateTable();

			//Add Entry
			Button button = FindViewById<Button>(Resource.Id.AddEntry);
			button.Click += delegate
			{
				Bill b = new Bill() { Name = "Car Gas", Amount= 15.67, Description = "to ottawa"  };
				dbs.InsertBillEntry(b);

			};

			//View Entry
			Button viewB = FindViewById<Button>(Resource.Id.Viewbtn);
			viewB.Click += delegate
			{
				var bills = dbs.GetAllBills();
				string s = "";
				foreach (var bill in bills)
				{
					s += bill.Name + "\n";
				}
				Toast.MakeText(this, s, ToastLength.Short).Show();

			};


		}
	}
}

