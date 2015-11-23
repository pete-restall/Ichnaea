using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventUsingInstanceMethodCallTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventUsingInstanceMethodCallTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventUsingInstanceMethodCall"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
