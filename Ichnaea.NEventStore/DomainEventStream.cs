using System;
using System.Collections.Generic;
using NEventStore;

namespace Restall.Ichnaea.NEventStore
{
	public class DomainEventStream<TAggregateRoot>: IDomainEventStream<TAggregateRoot>, IDisposable
	{
		private readonly List<IEventStream> eventStoreStreams = new List<IEventStream>();
		private readonly IStoreEvents eventStore;
		private readonly IPrePersistenceDomainEventTracker<TAggregateRoot> prePersistenceDomainEventTracker;
		private readonly AggregateRootIdGetter<TAggregateRoot> aggregateRootIdGetter;
		private readonly string bucketId;
		private readonly Converter<object, EventMessage> domainEventToPersistableConverter;

		public DomainEventStream(
			IStoreEvents eventStore,
			IPrePersistenceDomainEventTracker<TAggregateRoot> prePersistenceDomainEventTracker,
			AggregateRootIdGetter<TAggregateRoot> aggregateRootIdGetter,
			string bucketId,
			Converter<object, EventMessage> domainEventToPersistableConverter)
		{
			this.eventStore = eventStore;
			this.prePersistenceDomainEventTracker = prePersistenceDomainEventTracker;
			this.aggregateRootIdGetter = aggregateRootIdGetter;
			this.bucketId = bucketId;
			this.domainEventToPersistableConverter = domainEventToPersistableConverter;
		}

		public void CreateFrom(TAggregateRoot aggregateRoot)
		{
			var eventStoreStream = this.eventStore.CreateStream(this.bucketId, this.aggregateRootIdGetter(aggregateRoot));
			this.eventStoreStreams.Add(eventStoreStream);
			this.prePersistenceDomainEventTracker.SwitchTrackingToPersistentStore(
				aggregateRoot,
				(sender, args) => eventStoreStream.Add(this.domainEventToPersistableConverter(args)));
		}

		public TAggregateRoot Replay(string id)
		{
			throw new NotImplementedException();
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
