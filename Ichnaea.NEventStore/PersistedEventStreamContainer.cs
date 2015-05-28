using System;
using System.Collections.Generic;
using NEventStore;

namespace Restall.Ichnaea.NEventStore
{
	public abstract class PersistedEventStreamContainer: IDisposable
	{
		// TODO: EVENT STREAMS SHOULD BE WEAK REFERENCES, OTHERWISE THEY MAY LIVE FOR A VERY LONG TIME AFTER THE AGGREGATE ROOT HAS DIED...
		private readonly List<IEventStream> eventStoreStreams = new List<IEventStream>();

		protected void AddStream(IEventStream stream)
		{
			this.eventStoreStreams.Add(stream);
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
