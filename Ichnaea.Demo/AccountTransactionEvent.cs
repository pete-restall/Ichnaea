namespace Restall.Ichnaea.Demo
{
	public abstract class AccountTransactionEvent: AccountEvent
	{
		protected AccountTransactionEvent(decimal amount, string description)
		{
			this.Amount = amount;
			this.Description = description;
		}

		public decimal Amount { get; set; }

		public string Description { get; set; }
	}
}
