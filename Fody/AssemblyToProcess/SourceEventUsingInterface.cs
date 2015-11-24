using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	public class SourceEventUsingInterface
	{
		public interface ISomethingHappened
		{
			Guid Token { get; }
		}

		private class SomethingHappened: ISomethingHappened
		{
			public SomethingHappened(Guid token)
			{
				this.Token = token;
			}

			public Guid Token { get; }
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public void DoSomething(Guid token)
		{
			Source.Event.Of(new SomethingHappened(token));
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<ISomethingHappened> EventSource;
	}
}
