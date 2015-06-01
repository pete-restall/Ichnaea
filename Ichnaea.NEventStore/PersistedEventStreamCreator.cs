using System;
using NEventStore;

namespace Restall.Ichnaea.NEventStore
{
	public class PersistedEventStreamCreator<TAggregateRoot>: DisposableContainer
		where TAggregateRoot: class
	{
		private readonly IStoreEvents eventStore;
		private readonly string bucketId;
		private readonly IPrePersistenceDomainEventTracker<TAggregateRoot> prePersistenceDomainEventTracker;
		private readonly AggregateRootIdGetter<TAggregateRoot> aggregateRootIdGetter;
		private readonly Converter<object, EventMessage> domainEventToPersistedEventConverter;

		public PersistedEventStreamCreator(
			IStoreEvents eventStore,
			string bucketId,
			IPrePersistenceDomainEventTracker<TAggregateRoot> prePersistenceDomainEventTracker,
			AggregateRootIdGetter<TAggregateRoot> aggregateRootIdGetter,
			Converter<object, EventMessage> domainEventToPersistedEventConverter)
		{
			this.eventStore = eventStore;
			this.bucketId = bucketId;
			this.prePersistenceDomainEventTracker = prePersistenceDomainEventTracker;
			this.aggregateRootIdGetter = aggregateRootIdGetter;
			this.domainEventToPersistedEventConverter = domainEventToPersistedEventConverter;
		}

		public virtual void CreateFrom(TAggregateRoot aggregateRoot)
		{
			var eventStoreStream = this.eventStore.CreateStream(this.bucketId, this.aggregateRootIdGetter(aggregateRoot));
			base.AddDisposable(eventStoreStream);
			this.WriteFutureDomainEventsDirectlyToEventStore(aggregateRoot, eventStoreStream);
		}

		private void WriteFutureDomainEventsDirectlyToEventStore(TAggregateRoot aggregateRoot, IEventStream eventStoreStream)
		{
			this.prePersistenceDomainEventTracker.SwitchTrackingToPersistentStore(
				aggregateRoot,
				(sender, args) => eventStoreStream.Add(this.domainEventToPersistedEventConverter(args)));
		}
	}
}
