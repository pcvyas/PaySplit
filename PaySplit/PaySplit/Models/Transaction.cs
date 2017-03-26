using System;
using SQLite;

namespace PaySplit
{
	[Table("Transactions")]
	public class Transaction
	{
		public Transaction()
		{
			UID = Guid.NewGuid().ToString();
			Completed = false;
		}

	
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		[Unique]
		public string UID { get; set; } /* The UUID of the transaction */
		public string BillUID { get; set; } /* The ID of the bill object in the bill table */ 
		public string SenderEmail { get; set; } /* The Contact of the sender of the transaction request */
		public string ReceiverEmail { get; set; } /* The Contact of the receiver of the transaction request */
		public double Amount { get; set; } /* The amount owed by the sender */
		public bool Completed { get; set; } /* The status of the transaction */
	}
}