using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventUsingComplexObjectInitialiserTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventUsingComplexObjectInitialiserTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventUsingComplexObjectInitialiser"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameObjectInitialisedToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
