using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	public class SourceEventUsingStaticMethodCall
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public void DoSomething(Guid token)
		{
			Source.Event.Of(StaticMethodCall(token));
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static SomethingHappened StaticMethodCall(Guid token)
		{
			return new SomethingHappened(token);
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
