using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using NullGuard;

namespace Restall.Ichnaea
{
	[Serializable]
	[NullGuard(ValidationFlags.None)]
	public class DomainEventCannotBeReplayedException: Exception
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
		public DomainEventCannotBeReplayedException()
		{
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
		public DomainEventCannotBeReplayedException(string message): base(message)
		{
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
		public DomainEventCannotBeReplayedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DomainEventCannotBeReplayedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.DomainEvent = info.GetOrDefault<object>(nameof(this.DomainEvent), null);
		}

		public DomainEventCannotBeReplayedException(string message, object domainEvent)
			: base(message)
		{
			this.DomainEvent = domainEvent;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddOrDefault(nameof(this.DomainEvent), this.DomainEvent, x => x?.ToString());
		}

		public object DomainEvent { get; }
	}
}
