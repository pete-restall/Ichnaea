using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	public class SourceHomogenousEventFromTwoMethods
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public void DoSomething(Guid token)
		{
			Source.Event.Of(new SomethingHappened(token));
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public void AndAnotherThing(Guid token)
		{
			Source.Event.Of(new SomethingHappened(token));
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
