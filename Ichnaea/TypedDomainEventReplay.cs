using System;
using NullGuard;

namespace Restall.Ichnaea
{
	[NullGuard(ValidationFlags.None)]
	public abstract class TypedDomainEventReplay<TAggregateRoot, TDomainEvent>: IReplayDomainEvents<TAggregateRoot>
		where TAggregateRoot: class
		where TDomainEvent: class
	{
		private readonly bool isAggregateRootRequired;

		protected TypedDomainEventReplay(bool isAggregateRootRequired = true)
		{
			this.isAggregateRootRequired = isAggregateRootRequired;
		}

		public virtual bool CanReplay(TAggregateRoot aggregateRoot, object domainEvent)
		{
			return (!this.isAggregateRootRequired || aggregateRoot != null) && domainEvent is TDomainEvent;
		}

		public virtual TAggregateRoot Replay(TAggregateRoot aggregateRoot, object domainEvent)
		{
			if (aggregateRoot == null && this.isAggregateRootRequired)
				throw new ArgumentNullException(nameof(aggregateRoot));

			if (domainEvent == null)
				throw new ArgumentNullException(nameof(domainEvent));

			var castDomainEvent = domainEvent as TDomainEvent;
			if (castDomainEvent == null)
			{
				throw new DomainEventCannotBeReplayedException(
					"Could not replay Domain Event because it is not of type " + typeof(TDomainEvent) + "; " +
						"aggregateRoot=" + aggregateRoot +
						", domainEvent=" + domainEvent,
					domainEvent);
			}

			return this.Replay(aggregateRoot, castDomainEvent);
		}

		protected abstract TAggregateRoot Replay(TAggregateRoot aggregateRoot, TDomainEvent domainEvent);
	}
}
