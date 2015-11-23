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
		public void DoSomething_Called_ExpectCallToSourceEventHasNotBeenWovenSoThrowsNotImplementedException()
		{
			AggregateRoot.Invoking(x => ((dynamic) x).DoSomething(Guid.NewGuid()))
				.ShouldThrow<NotImplementedException>()
				.Where(exception => exception.Message.Contains("Source.Event") && exception.Message.Contains("Fody"));
		}
	}
}
