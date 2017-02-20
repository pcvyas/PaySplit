using System;
using SQLite;

namespace PaySplit
{
	[Table("Bill")]
	public class Bill
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Name { get; set;}
		public string Description { get; set;}
		public double Amount { get; set; }
		public string ImagePath { get; set; }
	}
}
