using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NEventStore;

namespace Restall.Ichnaea.NEventStore
{
	public class NEventStoreSessionStream: IEventStream
	{
		private readonly IDictionary<Guid, IEventStream> eventStreamsInSession;
		private readonly IEventStream eventStream;
		private readonly Guid sessionStreamId;

		public NEventStoreSessionStream(ConcurrentDictionary<Guid, IEventStream> eventStreamsInSession, IEventStream eventStream)
		{
			this.eventStreamsInSession = eventStreamsInSession;
			this.eventStream = eventStream;
			this.sessionStreamId = Guid.NewGuid();
			this.eventStreamsInSession.Add(this.sessionStreamId, this);
		}

		public void Dispose()
		{
			this.eventStreamsInSession.Remove(this.sessionStreamId);
			this.eventStream.Dispose();
		}

		public void Add(EventMessage uncommittedEvent)
		{
			this.eventStream.Add(uncommittedEvent);
		}

		public void CommitChanges(Guid commitId)
		{
			this.eventStream.CommitChanges(commitId);
		}

		public void ClearChanges()
		{
			this.eventStream.ClearChanges();
		}

		public string BucketId => this.eventStream.BucketId;

		public string StreamId => this.eventStream.StreamId;

		public int StreamRevision => this.eventStream.StreamRevision;

		public int CommitSequence => this.eventStream.CommitSequence;

		public ICollection<EventMessage> CommittedEvents => this.eventStream.CommittedEvents;

		public IDictionary<string, object> CommittedHeaders => this.eventStream.CommittedHeaders;

		public ICollection<EventMessage> UncommittedEvents => this.eventStream.UncommittedEvents;

		public IDictionary<string, object> UncommittedHeaders => this.eventStream.UncommittedHeaders;
	}
}
