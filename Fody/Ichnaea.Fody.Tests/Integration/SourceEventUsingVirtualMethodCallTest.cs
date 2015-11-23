using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventUsingVirtualMethodCallTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventUsingVirtualMethodCallTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventUsingVirtualMethodCall"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectOnlyCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
