using System;
using System.Runtime.Serialization;
using NullGuard;

namespace Restall.Ichnaea
{
	[Serializable]
	[NullGuard(ValidationFlags.None)]
	public class DomainEventStreamCannotBeReplayedException: Exception
	{
		public DomainEventStreamCannotBeReplayedException()
		{
		}

		public DomainEventStreamCannotBeReplayedException(string message): base(message)
		{
		}

		public DomainEventStreamCannotBeReplayedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DomainEventStreamCannotBeReplayedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.AggregateRootId = info.GetOrDefault<object>("AggregateRootId", null);
		}

		public DomainEventStreamCannotBeReplayedException(string message, object aggregateRootId)
			: base(message)
		{
			this.AggregateRootId = aggregateRootId;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddOrDefault("AggregateRootId", this.AggregateRootId, x => x != null ? x.ToString() : null);
		}

		public object AggregateRootId { get; private set; }
	}
}
