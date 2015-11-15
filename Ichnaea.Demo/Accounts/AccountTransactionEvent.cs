namespace Restall.Ichnaea.Demo.Accounts
{
	public abstract class AccountTransactionEvent: AccountEvent
	{
		protected AccountTransactionEvent(decimal amount, string description)
		{
			this.Amount = amount;
			this.Description = description;
		}

		public decimal Amount { get; }

		public string Description { get; }
	}
}
