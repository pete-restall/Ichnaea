using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class AggregateWithSingleEventTest: AggregateTest
	{
		private const string EventName = "EventSource";

		public AggregateWithSingleEventTest():
			base(ModuleWeaverFixture.AggregateFactory.CreateAggregateWithSingleEvent())
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}

		[Fact]
		public void DoSomething_CalledWhenEventHasNoSubscribers_ExpectNoExceptionIsThrown()
		{
			this.Invoking(x => ((dynamic) this.Aggregate).DoSomething(Guid.NewGuid())).ShouldNotThrow<Exception>();
		}

		[Fact]
		public void DoSomethingAsTwoSeparateSourceEventCalls_Called_ExpectBothCallsToSourceEventsAreWovenToRaiseCliEvents()
		{
			var tokens = new[] {Guid.NewGuid(), Guid.NewGuid()};
			this.Aggregate.MonitorEvents();
			((dynamic) this.Aggregate).DoSomethingAsTwoSeparateSourceEventCalls(tokens[0], tokens[1]);
			tokens.ForEach(token =>
				this.Aggregate.ShouldRaise(EventName).WithDomainEvent<object>(this.Aggregate, x => SomethingHappenedWithToken(x, token)));
		}

		[Fact]
		public void DoSomethingViaLocalVariable_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.DoSomethingViaLocalVariable(token));
		}

		[Fact]
		public void DoSomethingViaManipulatedLocalVariable_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.DoSomethingViaManipulatedLocalVariable(token));
		}
	}
}
