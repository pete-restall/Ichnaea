﻿using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class AggregateRootWithSingleEventTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public AggregateRootWithSingleEventTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootWithSingleEvent())
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
			this.Invoking(x => ((dynamic) this.AggregateRoot).DoSomething(Guid.NewGuid())).ShouldNotThrow<Exception>();
		}

		[Fact]
		public void DoSomethingAsTwoSeparateSourceEventCalls_Called_ExpectBothCallsToSourceEventsAreWovenToRaiseCliEvents()
		{
			var tokens = new[] {Guid.NewGuid(), Guid.NewGuid()};
			this.AggregateRoot.MonitorEvents();
			((dynamic) this.AggregateRoot).DoSomethingAsTwoSeparateSourceEventCalls(tokens[0], tokens[1]);
			tokens.ForEach(token =>
				this.AggregateRoot.ShouldRaise(EventName).WithDomainEvent<object>(this.AggregateRoot, x => SomethingHappenedWithToken(x, token)));
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