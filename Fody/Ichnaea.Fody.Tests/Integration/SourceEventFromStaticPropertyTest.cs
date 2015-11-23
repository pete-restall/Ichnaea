using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventFromStaticPropertyTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventFromStaticPropertyTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventFromStaticProperty"))
		{
		}

		[Fact]
		public void DoSomethingInPropertyGet_Called_ExpectCallToSourceEventHasNotBeenWovenSoAnExceptionIsThrown()
		{
			this.AggregateRoot.Invoking(x => ((dynamic) x).DoSomethingInPropertyGet(Guid.NewGuid())).ShouldThrowFodyNotWovenSourceOfException();
		}

		[Fact]
		public void DoSomethingInPropertySet_Called_ExpectCallToSourceEventHasNotBeenWovenSoAnExceptionIsThrown()
		{
			this.AggregateRoot.Invoking(x => ((dynamic) x).DoSomethingInPropertySet(Guid.NewGuid())).ShouldThrowFodyNotWovenSourceOfException();
		}
	}
}
