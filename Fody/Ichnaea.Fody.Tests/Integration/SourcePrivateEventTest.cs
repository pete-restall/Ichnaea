using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourcePrivateEventTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourcePrivateEventTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourcePrivateEvent"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallSourcesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
