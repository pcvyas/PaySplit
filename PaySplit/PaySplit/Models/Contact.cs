using System;
using SQLite;

namespace PaySplit
{
	[Table("Contact")]
	public class Contact
	{
		public Contact()
		{
			UID = Guid.NewGuid().ToString();
		}

		public Contact(string customUID)
		{
			UID = customUID;
		}

		public Contact(string name, string email, string customUID)
		{
			FullName = name;
			Email = email;
			UID = customUID;
		}

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		[Unique]
		public string UID { get; set; } /* The UID generated that identifies a specific person */		
		public string FullName { get; set; } /* The users full contact name */
		[Unique]
		public string Email { get; set; } /* The users full email (for sending payments) */
	}
}