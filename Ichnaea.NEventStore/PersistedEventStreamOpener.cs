using System;
using System.Linq;
using NEventStore;

namespace Restall.Ichnaea.NEventStore
{
	public class PersistedEventStreamOpener<TAggregateRoot>: DisposableContainer
		where TAggregateRoot: class
	{
		private readonly IStoreEvents eventStore;
		private readonly string bucketId;
		private readonly Converter<object, string> aggregateRootIdToStringConverter;
		private readonly IPostPersistenceDomainEventTracker<TAggregateRoot> postPersistenceDomainEventTracker;
		private readonly Converter<object, EventMessage> domainEventToPersistedEventConverter;
		private readonly PersistedEventToDomainEventReplayAdapter<TAggregateRoot> persistedEventReplay;

		public PersistedEventStreamOpener(
			IStoreEvents eventStore,
			string bucketId,
			Converter<object, string> aggregateRootIdToStringConverter,
			IPostPersistenceDomainEventTracker<TAggregateRoot> postPersistenceDomainEventTracker,
			Converter<object, EventMessage> domainEventToPersistedEventConverter,
			PersistedEventToDomainEventReplayAdapter<TAggregateRoot> persistedEventReplay)
		{
			this.eventStore = eventStore;
			this.bucketId = bucketId;
			this.aggregateRootIdToStringConverter = aggregateRootIdToStringConverter;
			this.postPersistenceDomainEventTracker = postPersistenceDomainEventTracker;
			this.domainEventToPersistedEventConverter = domainEventToPersistedEventConverter;
			this.persistedEventReplay = persistedEventReplay;
		}

		public virtual TAggregateRoot Replay(object aggregateRootId)
		{
			var eventStoreStream = this.eventStore.OpenStream(
				this.bucketId,
				this.aggregateRootIdToStringConverter(aggregateRootId),
				int.MinValue,
				int.MaxValue);

			base.AddDisposable(eventStoreStream);

			var aggregateRoot = eventStoreStream.CommittedEvents.Aggregate(default(TAggregateRoot), this.ReplaySinglePersistedEvent);
			if (aggregateRoot == null)
				throw new DomainEventStreamCannotBeReplayedException(); // TODO: ADD NICE MESSAGE (ONCE EXCEPTION IMPLEMENTED PROPERLY)

			this.WriteFutureDomainEventsDirectlyToEventStore(aggregateRoot, eventStoreStream);
			return aggregateRoot;
		}

		private TAggregateRoot ReplaySinglePersistedEvent(TAggregateRoot aggregateRoot, EventMessage persistedEvent)
		{
			if (!this.persistedEventReplay.CanReplay(aggregateRoot, persistedEvent))
				throw new DomainEventStreamCannotBeReplayedException(); // TODO: ADD NICE MESSAGE (ONCE EXCEPTION IMPLEMENTED PROPERLY)

			return this.persistedEventReplay.Replay(aggregateRoot, persistedEvent);
		}

		private void WriteFutureDomainEventsDirectlyToEventStore(TAggregateRoot aggregateRoot, IEventStream eventStoreStream)
		{
			this.postPersistenceDomainEventTracker.TrackToPersistentStore(
				aggregateRoot,
				(sender, args) => eventStoreStream.Add(this.domainEventToPersistedEventConverter(args)));
		}
	}
}
