using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceInternalEventTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceInternalEventTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceInternalEvent"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallSourcesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
