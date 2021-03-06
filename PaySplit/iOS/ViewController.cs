﻿using System;
using AssetsLibrary;
using Foundation;
using UIKit;

namespace PaySplit.iOS
{
	public partial class ViewController : UIViewController
	{
		int count = 1;
		GenDataService database;

		public ViewController(IntPtr handle) : base(handle)
		{
			//Generate or Initialize Database Path
			DataHelper dbPath = new DataHelper();
			dbPath.CreateDataBase("PaySplitDataDb.db3");

			//Initialize database service
			database = new GenDataService(dbPath.DBPath);

			//Create Table
			database.CreateTableIfNotExists();


			
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


			// Perform any additional setup after loading the view, typically from a nib.
			Button.AccessibilityIdentifier = "myButton";
			Button.TouchUpInside += delegate
			{
				var title = string.Format("{0} clicks!", count++);
				Button.SetTitle(title, UIControlState.Normal);
			};

			//Sample for adding items
			addItem.TouchUpInside += delegate 
			{
				Bill b = new Bill() { Name = "Car Gas", Amount = 15.67, Description = "to ottawa" };
				database.InsertBillEntry(b);
				UIStoryboard board = UIStoryboard.FromName("Main", null);
				UIViewController ctrl = (UIViewController)board.InstantiateViewController("CreateBill");
				this.PresentViewController(ctrl, true, null);

			};


			CameraService cs = new CameraService(imageVW, this);
			//cs.StartCamera();


			viewItem.TouchUpInside += delegate
			{
				UIStoryboard board = UIStoryboard.FromName("Main", null);
				UIViewController ctrl = (UIViewController)board.InstantiateViewController("ViewBills");
				this.PresentViewController(ctrl, true, null);
				/*
				var bills = database.GetAllBills();
				string s = "";
				foreach (var bill in bills)
				{
					s += bill.Name + "\n";
				}

				Console.WriteLine(s);
				*/
				//Just for testing (Need a seperate listener
				//cs.StartCamera();
			};


		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.		
		}
	}



}
