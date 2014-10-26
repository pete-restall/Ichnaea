using System;
using NEventStore;

namespace Restall.Ichnaea.NEventStore
{
	public class DomainEventStream<TAggregateRoot>: IDomainEventStream<TAggregateRoot>
	{
		private readonly IStoreEvents eventStore;
		private readonly IPrePersistenceDomainEventTracker<TAggregateRoot> prePersistenceDomainEventTracker;
		private readonly Func<TAggregateRoot, string> aggregateRootIdGetter;
		private readonly Func<TAggregateRoot, string> bucketIdGetter;

		public DomainEventStream(
			IStoreEvents eventStore,
			IPrePersistenceDomainEventTracker<TAggregateRoot> prePersistenceDomainEventTracker,
			Func<TAggregateRoot, string> aggregateRootIdGetter,
			Func<TAggregateRoot, string> bucketIdGetter)
		{
			this.eventStore = eventStore;
			this.prePersistenceDomainEventTracker = prePersistenceDomainEventTracker;
			this.aggregateRootIdGetter = aggregateRootIdGetter;
			this.bucketIdGetter = bucketIdGetter;
		}

		public void CreateFrom(TAggregateRoot aggregateRoot)
		{
			this.prePersistenceDomainEventTracker.SwitchTrackingToPersistentStore(aggregateRoot, (sender, args) => { });
			this.eventStore.CreateStream(this.bucketIdGetter(aggregateRoot), this.aggregateRootIdGetter(aggregateRoot));
		}

		public TAggregateRoot Replay(string id)
		{
			throw new NotImplementedException();
		}
	}
}
