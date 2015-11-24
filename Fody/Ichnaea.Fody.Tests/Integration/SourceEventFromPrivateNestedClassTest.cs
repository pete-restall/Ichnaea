using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventFromPrivateNestedClassTest: AggregateRootTest
	{
		public SourceEventFromPrivateNestedClassTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventFromPrivateNestedClass"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventHasNotBeenWovenSoAnExceptionIsThrown()
		{
			this.AggregateRoot.Invoking(x => ((dynamic) x).DoSomething(Guid.NewGuid())).ShouldThrowFodyNotWovenSourceOfException();
		}
	}
}
