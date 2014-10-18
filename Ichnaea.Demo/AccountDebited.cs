namespace Restall.Ichnaea.Demo
{
	public class AccountDebited: AccountTransactionEvent
	{
		public AccountDebited(decimal amount, string description):
			base(amount, description)
		{
		}
	}
}
