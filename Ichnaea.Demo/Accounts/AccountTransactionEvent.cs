namespace Restall.Ichnaea.Demo.Accounts
{
	public abstract class AccountTransactionEvent: AccountEvent
	{
		protected AccountTransactionEvent(decimal amount, string description)
		{
			this.Amount = amount;
			this.Description = description;
		}

		protected AccountTransactionEvent() { }

		public decimal Amount { get; private set; }

		public string Description { get; private set; }
	}
}
