namespace Restall.Ichnaea.Demo.Accounts.Replay
{
	public class BalanceDebitedReplay: TypedDomainEventReplay<Account, BalanceDebited>
	{
		protected override Account Replay(Account aggregateRoot, BalanceDebited domainEvent)
		{
			aggregateRoot.Debit(domainEvent.Amount, domainEvent.Description);
			return aggregateRoot;
		}
	}
}
