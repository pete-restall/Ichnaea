using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	using ExceptionTest = ExceptionTest<AggregateRootIdPropertyNotFoundException, AggregateRootIdPropertyNotFoundExceptionTest.DefaultExceptionProperties>;

	public class AggregateRootIdPropertyNotFoundExceptionTest: ExceptionTest
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global")]
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
		public void Constructor_CalledWithNullAggregateRootType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new AggregateRootIdPropertyNotFoundException(null, DummyType(), DummyPropertyName());
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootType");
		}

		private static Type DummyType()
		{
			return Substitute.For<object>().GetType();
		}

		private static string DummyPropertyName()
		{
			return StringGenerator.AnyNonNull();
		}

		[Fact]
		public void Constructor_CalledWithNullAggregateRootIdType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new AggregateRootIdPropertyNotFoundException(DummyType(), null, DummyPropertyName());
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootIdType");
		}

		[Fact]
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
	}
}
