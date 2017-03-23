using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;
using SQLite;


namespace PaySplit
{
	public class GenDataService
	{
		
		public string DBPath;

		private const string FIREBASE_URL = "https://paysplit-6daf0.firebaseio.com/";


		public GenDataService(string path)
		{
			this.DBPath = path;
		}

		//Firebase 
		/************************
		//Cloud DataBase Wrappers
		*************************/

		string GetTableName(Type t)
		{
			string stype = t.ToString();
			int pos = stype.LastIndexOf('.') + 1;
			return stype.Substring(pos, stype.Length - pos);
		}


		//Insert an Item
		public async void FBInsertItem<T>(T item)
		{
			//Table name
			string tableName = GetTableName(typeof(T));

			//FireBase Test
			FirebaseClient fbDatabase = new FirebaseClient(FIREBASE_URL);
			await fbDatabase.Child(tableName).PostAsync<T>(item);
		}

		//GetAll Items
		public async Task<List<T>> FBGetItems<T>()
		{
			//Table name
			string tableName = GetTableName(typeof(T));

			//FireBase Test
			FirebaseClient fbDatabase = new FirebaseClient(FIREBASE_URL);
			var entryList = await fbDatabase.Child(tableName).OnceAsync<T>();

			List<T> returnList = new List<T>();
			foreach (var entry in entryList)
			{
				returnList.Add(entry.Object);
			}
			return returnList;
		}

		//Get Bills by Category
		public async Task<List<Bill>> FBGetBillsByCategory(string cat)
		{
			//Table name
			string tableName = GetTableName(typeof(Bill));

			//FireBase Test
			FirebaseClient fbDatabase = new FirebaseClient(FIREBASE_URL);
			var entryList = await fbDatabase.Child(tableName) .OnceAsync<Bill>();
			((List<FirebaseObject<Bill>>)entryList).FindAll(o => o.Object.Category == cat);

			List<Bill> returnList = new List<Bill>();
			foreach (var entry in entryList)
			{
				returnList.Add(entry.Object);
			}
			return returnList;
		}

		//Get all contact names
		public async Task<List<string>> FBGetAllContactNames()
		{
			//Table name
			string tableName = GetTableName(typeof(Contact));

			//FireBase Test
			FirebaseClient fbDatabase = new FirebaseClient(FIREBASE_URL);
			var entryList = await fbDatabase.Child(tableName).OnceAsync<Contact>();

			List<string> returnList = new List<string>();
			foreach (var entry in entryList)
			{
				returnList.Add(entry.Object.FullName);
			}
			return returnList;
		}

		//Get Bill by ID
		public async Task<T> FBFindByUID<T>(string uid)
		{
			//Table name
			string tableName = GetTableName(typeof(T));

			//FireBase Test
			FirebaseClient fbDatabase = new FirebaseClient(FIREBASE_URL);
			var entryList = await fbDatabase.Child(tableName).OnceAsync<T>();

			return ((List<FirebaseObject<T>>)entryList).Find((obj) => ((dynamic)(obj.Object)).UID == uid).Object;
		}


		public async void FBDeleteItem<T>(T item)
		{
			//Table name
			string tableName = GetTableName(typeof(T));

			//FireBase Test
			FirebaseClient fbDatabase = new FirebaseClient(FIREBASE_URL);
			await fbDatabase.Child(tableName).Client.Child(((dynamic)item).UID).DeleteAsync();
		}

		public async void FBUpdateItem<T>(T item)
		{
			//Table name
			string tableName = GetTableName(typeof(T));

			//FireBase Test
			FirebaseClient fbDatabase = new FirebaseClient(FIREBASE_URL);
			//await fbDatabase.Child(tableName).Client.Child(((dynamic)item).UID).DeleteAsync();
			await fbDatabase.Child(tableName).PutAsync<T>(item);
		}

		/*---------------------------
		//END Cloud DataBase Wrappers
		-----------------------------*/

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

        public Bill getBillByUid(string uid)
        {
            Bill b = new Bill();
            try
            {
                if (DBPath == null)
                {
                    throw new Exception("Database does't exist!");
                }
                SQLiteConnection db = new SQLiteConnection(DBPath);
                b = db.Find<Bill>(uid);
                db.Close();
            }
            catch
            {
                return null;
            }
            return b;
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
				var contactTableQuery = db.Table<Contact>();
				foreach (Contact c in contactTableQuery)
				{
					if (c.Id.Equals(1))
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

		public Contact getContactByUID(string UID)
		{
			Contact c = new Contact();
			try
			{
				if (DBPath == null)
				{
					throw new Exception("Database does't exist!");
				}
				SQLiteConnection db = new SQLiteConnection(DBPath);
				c = db.Find<Contact>(UID);
				db.Close();
			}
			catch
			{
				return null;
			}
			return c;
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
        public bool SaveBillEntry(string uid, Bill b)
        {
            try
            {
                if (DBPath == null)
                {
                    throw new Exception("Database does't exist!");
                }
                SQLiteConnection db = new SQLiteConnection(DBPath);
                Bill bill = db.Find<Bill>(uid);
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
				Contact contact = db.Find<Contact>(c.UID);
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
