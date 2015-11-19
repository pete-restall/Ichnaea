using System;
using NullGuard;

namespace Restall.Ichnaea.Demo.Accounts.Replay
{
	// TODO: CREATE ABSTRACT BASE CLASS (IN ICHNAEA ASSEMBLY) FOR STRONGLY TYPED DOMAIN EVENTS
	public class BalanceCreditedReplay: IReplayDomainEvents<Account>
	{
		public bool CanReplay([AllowNull] Account aggregateRoot, object domainEvent)
		{
			return aggregateRoot != null && domainEvent is BalanceCreditedReplay;
		}

		public Account Replay([AllowNull] Account aggregateRoot, object domainEvent)
		{
			if (!this.CanReplay(aggregateRoot, domainEvent))
				throw new ArgumentException("Cannot replay Domain Event of type " + domainEvent.GetType(), nameof(domainEvent));

			var balanceCredited = (BalanceCredited) domainEvent;
			aggregateRoot.Credit(balanceCredited.Amount, balanceCredited.Description);
			return aggregateRoot;
		}
	}
}
