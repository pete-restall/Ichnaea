using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Unit
{
	public class TypeWeaverTest
	{
		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new TypeWeaver(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("type");
		}
	}
}
