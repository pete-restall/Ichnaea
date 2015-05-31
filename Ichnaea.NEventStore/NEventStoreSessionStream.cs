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

		public string BucketId { get { return this.eventStream.BucketId; } }

		public string StreamId { get { return this.eventStream.StreamId; } }

		public int StreamRevision { get { return this.eventStream.StreamRevision; } }

		public int CommitSequence { get { return this.eventStream.CommitSequence; } }

		public ICollection<EventMessage> CommittedEvents { get { return this.eventStream.CommittedEvents; } }

		public IDictionary<string, object> CommittedHeaders { get { return this.eventStream.CommittedHeaders; } }

		public ICollection<EventMessage> UncommittedEvents { get { return this.eventStream.UncommittedEvents; } }

		public IDictionary<string, object> UncommittedHeaders { get { return this.eventStream.UncommittedHeaders; } }
	}
}
