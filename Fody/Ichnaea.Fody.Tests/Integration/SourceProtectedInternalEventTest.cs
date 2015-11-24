using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceProtectedInternalEventTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceProtectedInternalEventTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceProtectedInternalEvent"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallSourcesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
