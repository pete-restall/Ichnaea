using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventFromInstancePropertyTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventFromInstancePropertyTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventFromInstanceProperty"))
		{
		}

		[Fact]
		public void DoSomethingInPropertyGet_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.DoSomethingInPropertyGet(token));
		}

		[Fact]
		public void DoSomethingInPropertySet_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.DoSomethingInPropertySet(token));
		}
	}
}
