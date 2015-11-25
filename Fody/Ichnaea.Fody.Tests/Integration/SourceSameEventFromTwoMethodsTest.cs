using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceSameEventFromTwoMethodsTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceSameEventFromTwoMethodsTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceSameEventFromTwoMethods"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallSourcesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}

		[Fact]
		public void AndAnotherThing_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallSourcesDomainEventWithSameToken(EventName, (x, token) => x.AndAnotherThing(token));
		}
	}
}
