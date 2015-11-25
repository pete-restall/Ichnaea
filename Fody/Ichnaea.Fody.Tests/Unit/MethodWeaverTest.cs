using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Unit
{
	public class MethodWeaverTest
	{
		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new MethodWeaver(null, DummyEventSourcingMethods());
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("method");
		}

		private static EventSourcingMethod[] DummyEventSourcingMethods()
		{
			return new EventSourcingMethod[0];
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullEventSourcingMethods_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new MethodWeaver(CecilTestDoubles.DummyMethodDefinition(), null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventSourcingMethods");
		}
	}
}
