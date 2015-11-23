using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	public class SourceEventUsingInstanceMethodCall
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public void DoSomething(Guid token)
		{
			Source.Event.Of(this.MethodCall(token));
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local", Justification = CodeAnalysisJustification.StubForTesting)]
		private SomethingHappened MethodCall(Guid token)
		{
			return new SomethingHappened(token);
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
