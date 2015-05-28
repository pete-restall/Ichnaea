using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Restall.Ichnaea
{
	public class InMemoryDomainEventTracker<TAggregateRoot>: IDomainEventTracker<TAggregateRoot>, IPrePersistenceDomainEventTracker<TAggregateRoot>, IDisposable
		where TAggregateRoot: class
	{
		private readonly List<DomainEventFunnel> funnels = new List<DomainEventFunnel>();

		// TODO: PROBABLY A TABLE OF Queue<object> ONCE THE PrePersistence INTERFACE IS IMPLEMENTED
		private ConditionalWeakTable<TAggregateRoot, List<object>> aggregateToDomainEventsMap =
			new ConditionalWeakTable<TAggregateRoot, List<object>>();

		public void AggregateRootCreated(TAggregateRoot aggregateRoot, object domainEvent)
		{
			List<object> domainEvents;
			if (this.aggregateToDomainEventsMap.TryGetValue(aggregateRoot, out domainEvents))
				throw new AggregateRootAlreadyBeingTrackedException();

			domainEvents = new List<object> {domainEvent};
			this.funnels.Add(new DomainEventFunnel(aggregateRoot, (sender, args) => domainEvents.Add(args)));
			this.aggregateToDomainEventsMap.Add(aggregateRoot, domainEvents);
		}

		public IEnumerable<object> GetSourcedDomainEventsFor(TAggregateRoot aggregateRoot)
		{
			List<object> domainEvents;
			if (this.aggregateToDomainEventsMap.TryGetValue(aggregateRoot, out domainEvents))
				return domainEvents;

			return new object[0];
		}

		void IPrePersistenceDomainEventTracker<TAggregateRoot>.SwitchTrackingToPersistentStore(
			TAggregateRoot aggregateRoot, Source.Of<object> persistentObserver)
		{
			// TODO: Method needs writing
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			this.funnels.ForEach(x => x.Dispose());
			this.funnels.Clear();
			this.aggregateToDomainEventsMap = new ConditionalWeakTable<TAggregateRoot, List<object>>();
		}
	}
}
