using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	public class SourceEventUsingManipulatedLocalVariable
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public SomethingHappened DoSomething(Guid token)
		{
			var domainEventAfterSwap = new SomethingHappened(Guid.NewGuid());
			var somethingToCauseManipulation = new SomethingHappened(token);
			Interlocked.Exchange(ref domainEventAfterSwap, somethingToCauseManipulation);
			Source.Event.Of(domainEventAfterSwap);
			return somethingToCauseManipulation;
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
