namespace Restall.Ichnaea.Demo.Accounts
{
	public class OverdraftAdjusted: AccountEvent
	{
		public OverdraftAdjusted(decimal amount)
		{
			this.Amount = amount;
		}

		public decimal Amount { get; }
	}
}
