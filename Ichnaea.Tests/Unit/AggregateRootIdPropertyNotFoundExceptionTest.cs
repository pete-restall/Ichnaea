using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	using ExceptionTest = ExceptionTest<AggregateRootIdPropertyNotFoundException, AggregateRootIdPropertyNotFoundExceptionTest.DefaultExceptionProperties>;

	public class AggregateRootIdPropertyNotFoundExceptionTest: ExceptionTest
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ReflectivePropertyComparison)]
		public new class ExceptionProperties: ExceptionTest.ExceptionProperties
		{
			public string PropertyName { get; set; }

			public Type AggregateRootType { get; set; }

			public Type AggregateRootIdType { get; set; }
		}

		public class DefaultExceptionProperties: ExceptionProperties
		{
			public DefaultExceptionProperties()
			{
				this.Message = ExceptionMessage.Default<AggregateRootIdPropertyNotFoundException>();
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullAggregateRootType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new AggregateRootIdPropertyNotFoundException(null, DummyType(), DummyPropertyName());
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootType");
		}

		private static Type DummyType()
		{
			return TypeGenerator.AnyReflectable();
		}

		private static string DummyPropertyName()
		{
			return StringGenerator.AnyNonNull();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullAggregateRootIdType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new AggregateRootIdPropertyNotFoundException(DummyType(), null, DummyPropertyName());
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootIdType");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullPropertyName_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new AggregateRootIdPropertyNotFoundException(DummyType(), DummyType(), null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("propertyName");
		}

		[Fact]
		public void AggregateRootType_Get_ExpectSameValueAsPassedToConstructor()
		{
			var aggregateRootType = DummyType();
			new AggregateRootIdPropertyNotFoundException(aggregateRootType, DummyType(), DummyPropertyName())
				.AggregateRootType.Should().BeSameAs(aggregateRootType);
		}

		[Fact]
		public void AggregateRootIdType_Get_ExpectSameValueAsPassedToConstructor()
		{
			var aggregateRootIdType = DummyType();
			new AggregateRootIdPropertyNotFoundException(DummyType(), aggregateRootIdType, DummyPropertyName())
				.AggregateRootIdType.Should().BeSameAs(aggregateRootIdType);
		}

		[Fact]
		public void PropertyName_Get_ExpectSameValueAsPassedToConstructor()
		{
			var propertyName = DummyPropertyName();
			new AggregateRootIdPropertyNotFoundException(DummyType(), DummyType(), propertyName)
				.PropertyName.Should().BeSameAs(propertyName);
		}

		[Fact]
		public void Message_Get_ExpectAggregateRootTypeAndIdTypeAndPropertyNameAreAllIncluded()
		{
			var aggregateRootType = DummyType();
			var aggregateRootIdType = DummyType();
			var propertyName = DummyPropertyName();
			new AggregateRootIdPropertyNotFoundException(aggregateRootType, aggregateRootIdType, propertyName).Message
				.Should().Contain(aggregateRootType.FullName)
				.And.Subject.Should().Contain(aggregateRootIdType.FullName)
				.And.Subject.Should().Contain(propertyName);
		}

		[Fact]
		public void ExpectAggregateRootTypeCanBeSerialised()
		{
			var aggregateRootType = DummyType();
			var deserialised = SerialiseAndDeserialise(new AggregateRootIdPropertyNotFoundException(aggregateRootType, DummyType(), DummyPropertyName()));
			deserialised.AggregateRootType.Should().Be(aggregateRootType);
		}

		[Fact]
		public void ExpectNullAggregateRootTypeIsSerialisedAsNull()
		{
			var deserialised = SerialiseAndDeserialise(new AggregateRootIdPropertyNotFoundException());
			deserialised.AggregateRootType.Should().BeNull();
		}

		[Fact]
		public void ExpectAggregateRootIdTypeCanBeSerialised()
		{
			var aggregateRootIdType = DummyType();
			var deserialised = SerialiseAndDeserialise(new AggregateRootIdPropertyNotFoundException(DummyType(), aggregateRootIdType, DummyPropertyName()));
			deserialised.AggregateRootIdType.Should().Be(aggregateRootIdType);
		}

		[Fact]
		public void ExpectNullAggregateRootIdTypeIsSerialisedAsNull()
		{
			var deserialised = SerialiseAndDeserialise(new AggregateRootIdPropertyNotFoundException());
			deserialised.AggregateRootIdType.Should().BeNull();
		}

		[Fact]
		public void ExpectPropertyNameCanBeSerialised()
		{
			var propertyName = DummyPropertyName();
			var deserialised = SerialiseAndDeserialise(new AggregateRootIdPropertyNotFoundException(DummyType(), DummyType(), propertyName));
			deserialised.PropertyName.Should().Be(propertyName);
		}

		[Fact]
		public void ExpectNullPropertyNameIsSerialisedAsNull()
		{
			var deserialised = SerialiseAndDeserialise(new AggregateRootIdPropertyNotFoundException());
			deserialised.PropertyName.Should().BeNull();
		}
	}
}
