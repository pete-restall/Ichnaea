using System;
using System.Diagnostics.CodeAnalysis;
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

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
		public AggregateRootNotBeingTrackedException(string message): base(message)
		{
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
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
