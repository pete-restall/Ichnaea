using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class NoEventSourceTest: AggregateRootTest
	{
		public NoEventSourceTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("NoEventSource"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventHasNotBeenWovenSoAnExceptionIsThrown()
		{
			this.AggregateRoot.Invoking(x => ((dynamic) x).DoSomething(Guid.NewGuid())).ShouldThrowFodyNotWovenSourceOfException();
		}
	}
}
