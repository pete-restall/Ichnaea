using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventFromProtectedInternalNestedClassTest: AggregateRootTest
	{
		public SourceEventFromProtectedInternalNestedClassTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventFromProtectedInternalNestedClass"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventHasNotBeenWovenSoAnExceptionIsThrown()
		{
			this.AggregateRoot.Invoking(x => ((dynamic) x).DoSomething(Guid.NewGuid())).ShouldThrowFodyNotWovenSourceOfException();
		}
	}
}
