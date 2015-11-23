using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventUsingStaticMethodCallTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventUsingStaticMethodCallTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventUsingStaticMethodCall"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
