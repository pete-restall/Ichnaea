namespace Restall.Ichnaea.Demo.Accounts.Replay
{
	public class BalanceCreditedReplay: TypedDomainEventReplay<Account, BalanceCredited>
	{
		protected override Account Replay(Account aggregateRoot, BalanceCredited domainEvent)
		{
			aggregateRoot.Credit(domainEvent.Amount, domainEvent.Description);
			return aggregateRoot;
		}
	}
}
