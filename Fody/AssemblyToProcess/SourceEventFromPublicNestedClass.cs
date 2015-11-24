using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	public class SourceEventFromPublicNestedClass
	{
		public class NestedClass
		{
			private readonly Guid token;

			public NestedClass(Guid token)
			{
				this.token = token;
			}

			public void DoSomething()
			{
				Source.Event.Of(new SomethingHappened(this.token));
			}
		}

		public void DoSomething(Guid token)
		{
			new NestedClass(token).DoSomething();
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
