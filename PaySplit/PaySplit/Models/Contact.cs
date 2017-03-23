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

		/* Used when we generate the first contact (current user) and assign UID "0" */
		public Contact(string customUID)
		{
			UID = customUID;
		}

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string UID { get; set; } /* The UID generated that identifies a specific person */		
		public string FullName { get; set; } /* The users full contact name */
		public string Email { get; set; } /* The users full email (for sending payments) */
		public string Password { get; set; } /* The users password */
	}
}