using System;
using System.Diagnostics.CodeAnalysis;
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

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
		public AggregateRootAlreadyBeingTrackedException(string message): base(message)
		{
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
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
