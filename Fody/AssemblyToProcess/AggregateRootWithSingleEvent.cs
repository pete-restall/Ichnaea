using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class AggregateRootWithSingleEvent
	{
		public void DoSomething(Guid token)
		{
			Source.Event.Of(new SomethingHappened(token));
		}

		public void DoSomethingAsTwoSeparateSourceEventCalls(Guid firstToken, Guid secondToken)
		{
			Source.Event.Of(new SomethingHappened(firstToken));
			Source.Event.Of(new SomethingHappened(secondToken));
		}

		public SomethingHappened DoSomethingViaLocalVariable(Guid token)
		{
			var domainEvent = new SomethingHappened(token);
			Source.Event.Of(domainEvent);
			return domainEvent;
		}

		public SomethingHappened DoSomethingViaManipulatedLocalVariable(Guid token)
		{
			var domainEventAfterSwap = new SomethingHappened(Guid.NewGuid());
			var somethingToCauseManipulation = new SomethingHappened(token);
			Interlocked.Exchange(ref domainEventAfterSwap, somethingToCauseManipulation);
			Source.Event.Of(domainEventAfterSwap);
			return somethingToCauseManipulation;
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global")]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
