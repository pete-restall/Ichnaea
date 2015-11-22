using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
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

		public void DoSomethingWithVirtualCalls(Guid token)
		{
			Source.Event.Of(new SomethingHappened(this.VirtualCall(token)));
		}

		protected virtual Guid VirtualCall(Guid token)
		{
			return token;
		}

		public void DoSomethingViaObjectInitialiserEvent(Guid token)
		{
			Source.Event.Of(new ObjectInitialiserSomethingHappened { Token = token });
		}

		public void DoSomethingViaComplexObjectInitialiserEvent(Guid token)
		{
			Source.Event.Of(new ObjectInitialiserSomethingHappened { Token = this.VirtualCall(token) });
		}

		public void DoSomethingViaInstanceMethodCall(Guid token)
		{
			Source.Event.Of(this.MethodCall(token));
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local", Justification = CodeAnalysisJustification.StubForTesting)]
		private SomethingHappened MethodCall(Guid token)
		{
			return new SomethingHappened(token);
		}

		public void DoSomethingViaStaticMethodCall(Guid token)
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
