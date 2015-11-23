using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceHomogenousEventFromTwoMethodsTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceHomogenousEventFromTwoMethodsTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceHomogenousEventFromTwoMethods"))
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
