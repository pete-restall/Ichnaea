using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventUsingObjectInitialiserTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventUsingObjectInitialiserTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventUsingObjectInitialiser"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameObjectInitialisedToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
