namespace Restall.Ichnaea.Demo.Accounts
{
	public class BalanceDebited: AccountTransactionEvent
	{
		public BalanceDebited(decimal amount, string description):
			base(amount, description)
		{
		}
	}
}
