using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventUsingInstanceFieldTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventUsingInstanceFieldTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventUsingInstanceField"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
