namespace Restall.Ichnaea.Demo.Accounts
{
	public class BalanceDebitRejected: AccountTransactionEvent
	{
		public BalanceDebitRejected(decimal amount, string description, string reason):
			base(amount, description)
		{
			this.Reason = reason;
		}

		public string Reason { get; }
	}
}
