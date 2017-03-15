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
		}

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Name { get; set;}
		public string Description { get; set;}
		public string Category { get; set; }
		public double Amount { get; set; }
		public string ImagePath { get; set; }
		public DateTime Date { get; set; }
        public DateTime LastEdited { get; set;  }
        public Contact Owner { get; set; }
	}
}
