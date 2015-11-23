using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	public class SourceEventUsingStaticField
	{
		private static SomethingHappened staticField;

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public void DoSomething(Guid token)
		{
			staticField = new SomethingHappened(token);
			Source.Event.Of(staticField);
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
