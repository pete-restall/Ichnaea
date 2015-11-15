namespace Restall.Ichnaea.Demo.Accounts
{
	public class OverdraftAdjustmentRejected: AccountEvent
	{
		public OverdraftAdjustmentRejected(decimal amount, string reason)
		{
			this.Amount = amount;
			this.Reason = reason;
		}

		public decimal Amount { get; }

		public string Reason { get; }
	}
}
