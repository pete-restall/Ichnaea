using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class SourceEventFromNonPublicMethodsTest: AggregateRootTest
	{
		private const string EventName = "EventSource";

		public SourceEventFromNonPublicMethodsTest():
			base(ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("SourceEventFromNonPublicMethods"))
		{
		}

		[Fact]
		public void SourceEventFromPrivateMethod_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.SourceEventFromPrivateMethod(token));
		}

		[Fact]
		public void SourceEventFromProtectedMethod_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.SourceEventFromProtectedMethod(token));
		}

		[Fact]
		public void SourceEventFromProtectedInternalMethod_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.SourceEventFromProtectedInternalMethod(token));
		}

		[Fact]
		public void SourceEventFromInternalMethod_Called_ExpectCallToSourceEventIsWovenToRaiseCliEvent()
		{
			this.ExpectDynamicCallRaisesDomainEventWithSameToken(EventName, (x, token) => x.SourceEventFromInternalMethod(token));
		}
	}
}
