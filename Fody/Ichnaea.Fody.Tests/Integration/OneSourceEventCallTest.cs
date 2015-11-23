using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class OneSourceEventCallTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public OneSourceEventCallTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("OneSourceEventCall"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallSourcesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}

		[Fact]
		public void DoSomething_CalledWhenEventHasNoSubscribers_ExpectNoExceptionIsThrown()
		{
			this.Invoking(x => ((dynamic) this.AggregateRoot).DoSomething(Guid.NewGuid())).ShouldNotThrow<Exception>();
		}
	}
}
