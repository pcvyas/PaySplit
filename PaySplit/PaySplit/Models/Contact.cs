using System;
using SQLite;

namespace PaySplit
{
	[Table("Contact")]
	public class Contact
	{
		public Contact()
		{

		}

		public Contact(string name, string email)
		{
			FullName = name;
			Email = email;
		}

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }	
		[Unique]
		public string Email { get; set; } /* The users full email (for sending payments) */
		public string FullName { get; set; } /* The users full contact name */
	}
}