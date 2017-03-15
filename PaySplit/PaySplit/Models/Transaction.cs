using SQLite;

namespace PaySplit
{
	[Table("Transactions")]
	public class Transaction
	{
		public Transaction()
		{
			Completed = false;
		}

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public Bill Bill { get; set; } /* The ID of the bill object in the bill table */ 
		public Contact Sender { get; set; } /* The Contact of the sender of the transaction request */
		public Contact Receiver { get; set; } /* The Contact of the receiver of the transaction request */
		public double Amount { get; set; } /* The amount owed by the sender */
		public bool Completed { get; set; } /* The status of the transaction */
	}
}