using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
				db.CreateTable<Transaction>();
				db.CreateTable<Contact>();
				db.Close();
			}
			catch (SQLiteException)
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

		//Insert a new Transaction
		public bool InsertTransactionEntry(Transaction t)
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteConnection db = new SQLiteConnection(DBPath);
				db.Insert(t);
				db.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool InsertTransactionEntries(List<Transaction> ts)
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteConnection db = new SQLiteConnection(DBPath);
				db.InsertAll(ts);
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

		//Get all Bills
		public List<Bill> GetBillsByCategory(string category)
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
					if (b.Category.Equals(category))
					{
						bs.Add(b);
					}
				}
				db.Close();

			}
			catch
			{
				return new List<Bill>();
			}
			return bs;
		}

        public Bill getBillByUID(string uid)
        {
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
					if ((b.UID).Equals(uid))
					{
						return b;
					}
				}
				db.Close();

			}
			catch
			{
				return null;
			}

			return null;
        }

		public List<Contact> GetAllContacts()
		{
			List<Contact> contacts = new List<Contact>();

			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}

				SQLiteConnection db = new SQLiteConnection(DBPath);
				var contactTableQuery = db.Table<Contact>();
				foreach (Contact c in contactTableQuery)
				{
					contacts.Add(c);
				}
				db.Close();

			}
			catch
			{
				return new List<Contact>();
			}

			return contacts;
		}

		public List<string> GetAllContactNames()
		{
			List<string> contacts = new List<string>();

			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}

				SQLiteConnection db = new SQLiteConnection(DBPath);
				var contactTableQuery = db.Table<Contact>();
				foreach (Contact c in contactTableQuery)
				{
					contacts.Add(c.FullName);
				}
				db.Close();

			}
			catch
			{
				return new List<string>();
			}

			return contacts;
		}

		public Contact getUserContactInformation()
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}

				SQLiteConnection db = new SQLiteConnection(DBPath);
				Contact c = db.Find<Contact>(1);
				if (c != null)
				{
					return c;
				}
				db.Close();
			}
			catch
			{
				return null;
			}

			return null;
		}

		public Contact getContactByEmail(string email)
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}

				SQLiteConnection db = new SQLiteConnection(DBPath);
				var contactTableQuery = db.Table<Contact>();
				foreach (Contact c in contactTableQuery)
				{
					if (c.Email.Equals(email))
					{
						return c;
					}
				}
				db.Close();
			}
			catch
			{
				return null;
			}

			return null;
		}

		public List<Transaction> getTransactionsForBill(string UID)
		{
			List<Transaction> transactions = new List<Transaction>();
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}

				SQLiteConnection db = new SQLiteConnection(DBPath);
				var transactionQuery = db.Table<Transaction>();
				foreach (Transaction t in transactionQuery)
				{
					if (t.BillUID.Equals(UID))
					{
						transactions.Add(t);
					}
				}
				db.Close();

			}
			catch
			{
				return null;
			}

			return transactions;
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

		public bool DeleteContact(Contact c)
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteConnection db = new SQLiteConnection(DBPath);
				db.Delete(c);
				db.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		/* Delete Operations */
		//Delete A Bill
		public bool DeleteBillAsync(Bill b)
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteAsyncConnection db = new SQLiteAsyncConnection(DBPath);
				db.DeleteAsync(b);
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


		/* Update Operations */

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

		public bool UpdateContactInformation(Contact c)
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteConnection db = new SQLiteConnection(DBPath);
				Contact contact = db.Find<Contact>(c.Id);
				if (contact != null)
				{
					contact = c;
					db.Update(contact);
				}
				db.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool UpdateUserContactInformation(Contact c)
		{
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteConnection db = new SQLiteConnection(DBPath);
				Contact contact = db.Find<Contact>(1);
				if (contact != null)
				{
					contact = c;
					db.Update(contact);
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
