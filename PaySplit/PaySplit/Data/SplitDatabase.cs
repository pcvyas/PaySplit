using System;
using SQLite;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PaySplit
{
	public class SplitDatabase
	{
		readonly SQLiteAsyncConnection database;

		public SplitDatabase(string dbPath)
		{
			database = new SQLiteAsyncConnection(dbPath);
			database.CreateTableAsync<Bill>().Wait();
		}

		public Task<List<Bill>> GetBillsAsync()
		{
			return database.Table<Bill>().ToListAsync();
		}


		public Task<Bill> GetItemAsync(int id)
		{
			return database.Table<Bill>().Where(i => i.ID == id).FirstOrDefaultAsync();
		}

		public Task<int> SaveItemAsync(Bill item)
		{
			if (item.ID != 0)
			{
				return database.UpdateAsync(item);
			}
			else {
				return database.InsertAsync(item);
			}
		}

		public Task<int> DeleteItemAsync(Bill item)
		{
			return database.DeleteAsync(item);
		}
	}
}
