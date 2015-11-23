using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class NoAggregateRootDecorationTest: AggregateRootTest
	{
		public NoAggregateRootDecorationTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("NoAggregateRootDecoration"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventHasNotBeenWovenSoAnExceptionIsThrown()
		{
			AggregateRoot.Invoking(x => ((dynamic) x).DoSomething(Guid.NewGuid())).ShouldThrowFodyNotWovenSourceOfException();
		}
	}
}
