namespace Restall.Ichnaea.Demo
{
	public class AccountDebitRejected: AccountTransactionEvent
	{
		public AccountDebitRejected(decimal amount, string description, string reason):
			base(amount, description)
		{
			this.Reason = reason;
		}

		public string Reason { get; set; }
	}
}
