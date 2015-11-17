using System;
using System.Runtime.Serialization;
using NullGuard;

namespace Restall.Ichnaea
{
	[Serializable]
	[NullGuard(ValidationFlags.None)]
	public class AggregateRootNotBeingTrackedException: Exception
	{
		public AggregateRootNotBeingTrackedException()
		{
		}

		public AggregateRootNotBeingTrackedException(string message): base(message)
		{
		}

		public AggregateRootNotBeingTrackedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected AggregateRootNotBeingTrackedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
