using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Unit
{
	public class EventSourcingMethodTest
	{
		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullEventDefinition_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new EventSourcingMethod(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventDefinition");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void CanReplaceCallToStub_CalledWithNullStubMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var method = new EventSourcingMethod(CecilTestDoubles.DummyEventDefinition());
			method.Invoking(x => x.CanReplaceCallToStub(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("stubMethod");
		}
	}
}
