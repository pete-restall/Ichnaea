using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventUsingInterfaceTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventUsingInterfaceTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventUsingInterface"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallSourcesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
