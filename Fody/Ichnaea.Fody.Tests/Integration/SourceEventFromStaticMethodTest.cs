using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventFromStaticMethodTest: AggregateRootTest
	{
		public SourceEventFromStaticMethodTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventFromStaticMethod"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventHasNotBeenWovenSoAnExceptionIsThrown()
		{
			this.AggregateRoot.Invoking(x => ((dynamic) x).DoSomething(Guid.NewGuid())).ShouldThrowFodyNotWovenSourceOfException();
		}
	}
}
