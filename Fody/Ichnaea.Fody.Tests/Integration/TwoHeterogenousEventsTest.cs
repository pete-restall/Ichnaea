using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class TwoHeterogenousEventsTest: AggregateRootTest
	{
		public TwoHeterogenousEventsTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("TwoHeterogenousEvents"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectFirstEventIsWovenToRaiseCliEvents()
		{
			this.ExpectDynamicCallSourcesDomainEventWithSameToken("FirstEventSource", (x, token) => x.DoSomething(token));
		}

		[Fact]
		public void DoSomething_Called_ExpectSecondEventIsWovenToRaiseCliEvents()
		{
			this.ExpectDynamicCallSourcesDomainEventWithSameObjectInitialisedToken("SecondEventSource", (x, token) => x.DoSomething(token));
		}
	}
}
