using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Mono.Cecil;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Unit
{
	public class MethodWeaverTest
	{
		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new MethodWeaver(null, DummyMethodDefinition());
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("method");
		}

		private static MethodDefinition DummyMethodDefinition()
		{
			return new MethodDefinition("SomeMethod", MethodAttributes.Public, DummyTypeReference());
		}

		private static TypeReference DummyTypeReference()
		{
			var moduleDefinition = ModuleDefinition.CreateModule("SomeModule", new ModuleParameters());
			return moduleDefinition.ImportReference(typeof(void));
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullEventRaisingMethod_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new MethodWeaver(DummyMethodDefinition(), null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventRaisingMethod");
		}
	}
}
