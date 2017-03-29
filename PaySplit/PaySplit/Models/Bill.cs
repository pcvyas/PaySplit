using System;
using SQLite;

namespace PaySplit
{
	[Table("Bill")]
	public class Bill
	{
		public Bill()
		{
			Date = DateTime.Now;
			UID = Guid.NewGuid().ToString();
		}

		public Bill(string uid)
		{
			Date = DateTime.Now;
			UID = uid;
		}

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		[Unique]
		public string UID { get; set; }
		public string Name { get; set;}
		public string Description { get; set;}
		public string Category { get; set; }
		public double Amount { get; set; }
		public string ImagePath { get; set; }
		public DateTime Date { get; set; }
        public DateTime LastEdited { get; set; }
        public string OwnerEmail { get; set; }
	}
}