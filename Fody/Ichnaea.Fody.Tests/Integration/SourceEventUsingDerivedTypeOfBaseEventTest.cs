using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventUsingDerivedTypeOfBaseEventTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventUsingDerivedTypeOfBaseEventTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventUsingDerivedTypeOfBaseEvent"))
		{
		}

		[Fact]
		public void SourceDerivedEvent_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDerivedDomainEventWithSameToken(EventName, (x, token) => x.SourceDerivedEvent(token));
		}

		[Fact]
		public void SourceCastDerivedEvent_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDerivedDomainEventWithSameToken(EventName, (x, token) => x.SourceCastDerivedEvent(token));
		}
	}
}
