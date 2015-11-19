namespace Restall.Ichnaea.Demo.Accounts.Replay
{
	public class OverdraftAdjustedReplay: TypedDomainEventReplay<Account, OverdraftAdjusted>
	{
		protected override Account Replay(Account aggregateRoot, OverdraftAdjusted domainEvent)
		{
			aggregateRoot.AdjustOverdraft(domainEvent.Amount);
			return aggregateRoot;
		}
	}
}
