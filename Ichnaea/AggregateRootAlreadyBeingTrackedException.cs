using System;
using System.Runtime.Serialization;
using NullGuard;

namespace Restall.Ichnaea
{
	[Serializable]
	[NullGuard(ValidationFlags.None)]
	public class AggregateRootAlreadyBeingTrackedException: Exception
	{
		public AggregateRootAlreadyBeingTrackedException()
		{
		}

		public AggregateRootAlreadyBeingTrackedException(string message): base(message)
		{
		}

		public AggregateRootAlreadyBeingTrackedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected AggregateRootAlreadyBeingTrackedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
