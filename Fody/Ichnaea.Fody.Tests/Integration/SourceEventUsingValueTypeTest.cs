using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventUsingValueTypeTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventUsingValueTypeTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventUsingValueType"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallSourcesDomainEventOfToken(EventName, (x, token) => x.DoSomething(token));
		}

		private void ExpectDynamicCallSourcesDomainEventOfToken(string eventFieldName, Action<dynamic, Guid> action)
		{
			var token = this.InvokeTokenActionOnMonitoredAggregateRoot<Guid>(eventFieldName, action);
			this.AggregateRoot
				.ShouldRaise(eventFieldName)
				.WithDomainEvent<Guid>(this.AggregateRoot, x => x == token);
		}
	}
}
