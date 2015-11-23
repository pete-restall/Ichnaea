using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class TwoConsecutiveSourceEventCallsTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public TwoConsecutiveSourceEventCallsTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("TwoConsecutiveSourceEventCalls"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectBothCallsToSourceEventAreWovenToRaiseCliEvents()
		{
			var tokens = new[] {Guid.NewGuid(), Guid.NewGuid()};
			this.AggregateRoot.MonitorEvents();
			((dynamic) this.AggregateRoot).DoSomething(tokens[0], tokens[1]);
			tokens.ForEach(token =>
				this.AggregateRoot.ShouldRaise(EventName).WithDomainEvent<object>(this.AggregateRoot, x => SomethingHappenedWithToken(x, token)));
		}
	}
}
