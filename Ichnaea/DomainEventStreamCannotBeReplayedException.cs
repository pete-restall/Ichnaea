using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using NullGuard;

namespace Restall.Ichnaea
{
	[Serializable]
	[NullGuard(ValidationFlags.None)]
	public class DomainEventStreamCannotBeReplayedException: Exception
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
		public DomainEventStreamCannotBeReplayedException()
		{
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
		public DomainEventStreamCannotBeReplayedException(string message): base(message)
		{
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
		public DomainEventStreamCannotBeReplayedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DomainEventStreamCannotBeReplayedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.AggregateRootId = info.GetOrDefault<object>(nameof(this.AggregateRootId), null);
		}

		public DomainEventStreamCannotBeReplayedException(string message, object aggregateRootId)
			: base(message)
		{
			this.AggregateRootId = aggregateRootId;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddOrDefault(nameof(this.AggregateRootId), this.AggregateRootId, x => x?.ToString());
		}

		public object AggregateRootId { get; }
	}
}
