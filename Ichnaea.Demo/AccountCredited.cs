namespace Restall.Ichnaea.Demo
{
	public class AccountCredited: AccountTransactionEvent
	{
		public AccountCredited(decimal amount, string description):
			base(amount, description)
		{
		}
	}
}
