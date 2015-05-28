using System;
using System.Linq;
using NEventStore;

namespace Restall.Ichnaea.NEventStore
{
 	// TODO: MAKE THE CLASS TAKE AN IPersistedEventStreamContainer (MULTIPLE IMPLEMENTATIONS - THREAD-SAFE OR NON-THREAD-SAFE ?) RATHER THAN INHERIT FROM IT; THIS MEANS MULTIPLE CLASSES CAN USE THE SAME CONTAINER, MEANING MORE TRANSACTIONAL OPPORTUNITIES (IE. Commit() CAN DO AN ENTIRE DOMAIN RATHER THAN A SINGLE AGGREGATE ROOT IN THAT DOMAIN)
	public class PersistedEventStreamOpener<TAggregateRoot>: PersistedEventStreamContainer
		where TAggregateRoot: class
	{
		private readonly IStoreEvents eventStore;
		private readonly string bucketId;
		private readonly IPostPersistenceDomainEventTracker<TAggregateRoot> postPersistenceDomainEventTracker;
		private readonly Converter<object, EventMessage> domainEventToPersistedEventConverter;
		private readonly PersistedEventToDomainEventReplayAdapter<TAggregateRoot> persistedEventReplay;

		public PersistedEventStreamOpener(
			IStoreEvents eventStore,
			string bucketId,
			IPostPersistenceDomainEventTracker<TAggregateRoot> postPersistenceDomainEventTracker,
			Converter<object, EventMessage> domainEventToPersistedEventConverter,
			PersistedEventToDomainEventReplayAdapter<TAggregateRoot> persistedEventReplay)
		{
			this.eventStore = eventStore;
			this.bucketId = bucketId;
			this.postPersistenceDomainEventTracker = postPersistenceDomainEventTracker;
			this.domainEventToPersistedEventConverter = domainEventToPersistedEventConverter;
			this.persistedEventReplay = persistedEventReplay;
		}

		public TAggregateRoot Replay(string aggregateRootId)
		{
			var eventStoreStream = this.eventStore.OpenStream(this.bucketId, aggregateRootId, int.MinValue, int.MaxValue);
			base.AddStream(eventStoreStream);

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
