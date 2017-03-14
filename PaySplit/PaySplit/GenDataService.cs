using System;
using System.Collections.Generic;
using SQLite;


namespace PaySplit
{
	public class GenDataService
	{
		
		public string DBPath;

		public GenDataService(string path)
		{
			this.DBPath = path;
		}	

		//Create Table
		public bool CreateTableIfNotExists()
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteConnection db = new SQLiteConnection(DBPath);	
				db.CreateTable<Bill>();
				//db.CreateTable<Transaction>();
				//db.CreateTable<Contact>();
				db.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		/* Insertion Operations */

		//Insert a new BillEntry
		public bool InsertBillEntry(Bill b)
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteConnection db = new SQLiteConnection(DBPath);
				db.Insert(b);
				db.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		//Insert a new Contact
		public bool InsertContactEntry(Contact c)
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteConnection db = new SQLiteConnection(DBPath);
				db.Insert(c);
				db.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		/* Retreival Operations */

		//Get all Bills
		public List<Bill> GetAllBills()
		{
			List<Bill> bs = new List<Bill>();

			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}

				SQLiteConnection db = new SQLiteConnection(DBPath);
				var bills = db.Table<Bill>();

				foreach (Bill b in bills)
				{
					bs.Add(b);
				}
				db.Close();

			}
			catch
			{
				return new List<Bill>();
			}
			return bs;
		}

        public Bill getBillById(int id)
        {
            Bill b = new Bill();
            try
            {
                if (DBPath == null)
                {
                    throw new Exception("Database does't exist!");
                }
                SQLiteConnection db = new SQLiteConnection(DBPath);
                b = db.Find<Bill>(id);
                db.Close();
            }
            catch
            {
                return null;
            }
            return b; ;
        }

		/* Delete Operations */
		//Delete A Bill
		public bool DeleteBill(Bill b)
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteConnection db = new SQLiteConnection(DBPath);
				db.Delete(b);
				db.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

        // Delete all bills
        public bool deleteAllBills()
        {
            try
            {
                if (DBPath == null)
                {
                    throw new Exception("Database does't exist!");
                }
                SQLiteConnection db = new SQLiteConnection(DBPath);
                db.DeleteAll<Bill>();
                db.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        //Save a BillEntry
        public bool SaveBillEntry(int id, Bill b)
        {
            try
            {
                if (DBPath == null)
                {
                    throw new Exception("Database does't exist!");
                }
                SQLiteConnection db = new SQLiteConnection(DBPath);
                Bill bill = db.Find<Bill>(id);
                if (bill != null)
                {
                    bill = b;
                    db.Update(bill);
                }
                db.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
