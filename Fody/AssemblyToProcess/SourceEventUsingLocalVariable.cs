using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	public class SourceEventUsingLocalVariable
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public SomethingHappened DoSomething(Guid token)
		{
			var domainEvent = new SomethingHappened(token);
			Source.Event.Of(domainEvent);
			return domainEvent;
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
