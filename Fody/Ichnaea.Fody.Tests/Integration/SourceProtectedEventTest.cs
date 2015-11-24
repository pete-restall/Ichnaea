using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceProtectedEventTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceProtectedEventTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceProtectedEvent"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallSourcesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
