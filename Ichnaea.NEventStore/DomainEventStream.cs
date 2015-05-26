using System;
using System.Collections.Generic;
using System.Linq;
using NEventStore;

namespace Restall.Ichnaea.NEventStore
{
	public class DomainEventStream<TAggregateRoot>: IDomainEventStream<TAggregateRoot>, IDisposable
		where TAggregateRoot: class
	{
		// TODO: EVENT STREAMS SHOULD BE WEAK REFERENCES, OTHERWISE THEY MAY LIVE FOR A VERY LONG TIME AFTER THE AGGREGATE ROOT HAS DIED...
		private readonly List<IEventStream> eventStoreStreams = new List<IEventStream>();
		private readonly IStoreEvents eventStore;
		private readonly IPrePersistenceDomainEventTracker<TAggregateRoot> prePersistenceDomainEventTracker;
		private readonly AggregateRootIdGetter<TAggregateRoot> aggregateRootIdGetter;
		private readonly string bucketId;
		private readonly Converter<object, EventMessage> domainEventToPersistedEventConverter;
		private readonly PersistedEventToDomainEventReplayAdapter<TAggregateRoot> persistedEventReplay;

		public DomainEventStream(
			IStoreEvents eventStore,
			IPrePersistenceDomainEventTracker<TAggregateRoot> prePersistenceDomainEventTracker,
			AggregateRootIdGetter<TAggregateRoot> aggregateRootIdGetter,
			string bucketId,
			Converter<object, EventMessage> domainEventToPersistedEventConverter,
			PersistedEventToDomainEventReplayAdapter<TAggregateRoot> persistedEventReplay)
		{
			this.eventStore = eventStore;
			this.prePersistenceDomainEventTracker = prePersistenceDomainEventTracker;
			this.aggregateRootIdGetter = aggregateRootIdGetter;
			this.bucketId = bucketId;
			this.domainEventToPersistedEventConverter = domainEventToPersistedEventConverter;
			this.persistedEventReplay = persistedEventReplay;
		}

		public void CreateFrom(TAggregateRoot aggregateRoot)
		{
			var eventStoreStream = this.eventStore.CreateStream(this.bucketId, this.aggregateRootIdGetter(aggregateRoot));
			this.eventStoreStreams.Add(eventStoreStream);
			this.WriteFutureDomainEventsDirectlyToEventStore(aggregateRoot, eventStoreStream);
		}

		private void WriteFutureDomainEventsDirectlyToEventStore(TAggregateRoot aggregateRoot, IEventStream eventStoreStream)
		{
			this.prePersistenceDomainEventTracker.SwitchTrackingToPersistentStore(
				aggregateRoot,
				(sender, args) => eventStoreStream.Add(this.domainEventToPersistedEventConverter(args)));
		}

		public TAggregateRoot Replay(string aggregateRootId)
		{
			var eventStoreStream = this.eventStore.OpenStream(this.bucketId, aggregateRootId, int.MinValue, int.MaxValue);
			this.eventStoreStreams.Add(eventStoreStream);

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

		public void Commit()
		{
			this.Commit(Guid.NewGuid());
		}

		public void Commit(Guid commitId)
		{
			this.eventStoreStreams.ForEach(x => x.CommitChanges(commitId));
		}

		public void ClearUncommitted()
		{
			this.eventStoreStreams.ForEach(x => x.ClearChanges());
		}

		public void Dispose()
		{
			this.eventStoreStreams.ForEach(x => x.Dispose());
			this.eventStoreStreams.Clear();
		}
	}
}
