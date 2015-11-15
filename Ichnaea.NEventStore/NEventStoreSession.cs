using System;
using System.Collections.Concurrent;
using NEventStore;
using NEventStore.Persistence;

namespace Restall.Ichnaea.NEventStore
{
	public class NEventStoreSession: IStoreEvents
	{
		private readonly IStoreEvents eventStore;
		private readonly ConcurrentDictionary<Guid, IEventStream> eventStreams;

		public NEventStoreSession(IStoreEvents eventStore)
		{
			this.eventStore = eventStore;
			this.eventStreams = new ConcurrentDictionary<Guid, IEventStream>();
		}

		public IEventStream CreateStream(string bucketId, string streamId)
		{
			return new NEventStoreSessionStream(this.eventStreams, this.eventStore.CreateStream(bucketId, streamId));
		}

		public IEventStream OpenStream(string bucketId, string streamId, int minRevision, int maxRevision)
		{
			return new NEventStoreSessionStream(this.eventStreams, this.eventStore.OpenStream(bucketId, streamId, minRevision, maxRevision));
		}

		public IEventStream OpenStream(ISnapshot snapshot, int maxRevision)
		{
			return new NEventStoreSessionStream(this.eventStreams, this.eventStore.OpenStream(snapshot, maxRevision));
		}

		public void StartDispatchScheduler()
		{
			this.eventStore.StartDispatchScheduler();
		}

		public void Dispose()
		{
			this.eventStreams.ForEach(stream => stream.Value.Dispose());
		}

		public void Commit()
		{
			this.Commit(Guid.NewGuid());
		}

		public void Commit(Guid commitId)
		{
			this.eventStreams.ForEach(stream => stream.Value.CommitChanges(commitId));
		}

		public void ClearUncommitted()
		{
			this.eventStreams.ForEach(stream => stream.Value.ClearChanges());
		}

		public IPersistStreams Advanced => this.eventStore.Advanced;
	}
}
