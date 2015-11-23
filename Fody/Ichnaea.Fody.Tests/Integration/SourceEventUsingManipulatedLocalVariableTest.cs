using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventUsingManipulatedLocalVariableTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventUsingManipulatedLocalVariableTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventUsingManipulatedLocalVariable"))
		{
		}

		[Fact]
		public void DoSomething_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.DoSomething(token));
		}
	}
}
