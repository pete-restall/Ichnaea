using System;
using System.Linq;
using NEventStore;

namespace Restall.Ichnaea.NEventStore
{
	public class PersistedEventStreamOpener<TAggregateRoot, TAggregateRootId>: DisposableContainer
		where TAggregateRoot: class
	{
		private readonly IStoreEvents eventStore;
		private readonly string bucketId;
		private readonly Converter<TAggregateRootId, string> aggregateRootIdToStringConverter;
		private readonly IPostPersistenceDomainEventTracker<TAggregateRoot> postPersistenceDomainEventTracker;
		private readonly Converter<object, EventMessage> domainEventToPersistedEventConverter;
		private readonly PersistedEventToDomainEventReplayAdapter<TAggregateRoot> persistedEventReplay;

		public PersistedEventStreamOpener(
			IStoreEvents eventStore,
			string bucketId,
			Converter<TAggregateRootId, string> aggregateRootIdToStringConverter,
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

		public virtual TAggregateRoot Replay(TAggregateRootId aggregateRootId)
		{
			var streamId = this.aggregateRootIdToStringConverter(aggregateRootId);
			var eventStoreStream = this.eventStore.OpenStream(
				this.bucketId,
				streamId,
				int.MinValue,
				int.MaxValue);

			this.AddDisposable(eventStoreStream);

			var aggregateRoot = eventStoreStream.CommittedEvents.Aggregate(
				default(TAggregateRoot),
				(aggregateRootUnderConstruction, persistedEvent) =>
					this.ReplaySinglePersistedEventOrThrow(
						aggregateRootUnderConstruction,
						persistedEvent,
						failedEvent => new DomainEventStreamCannotBeReplayedException(
							"Could not replay persisted Domain Event; " +
								"bucket=" + this.bucketId +
								", stream=" + streamId +
								", event=" + failedEvent.Body,
							aggregateRootId)));

			if (aggregateRoot == null)
			{
				throw new DomainEventStreamCannotBeReplayedException(
					"Domain Event replay resulted in null Aggregate Root; bucket=" + this.bucketId + ", stream=" + streamId,
					aggregateRootId);
			}

			this.WriteFutureDomainEventsDirectlyToEventStore(aggregateRoot, eventStoreStream);
			return aggregateRoot;
		}

		private TAggregateRoot ReplaySinglePersistedEventOrThrow(TAggregateRoot aggregateRoot, EventMessage persistedEvent, Func<EventMessage, Exception> exception)
		{
			if (!this.persistedEventReplay.CanReplay(aggregateRoot, persistedEvent))
				throw exception(persistedEvent);

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
