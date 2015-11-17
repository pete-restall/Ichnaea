using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	using ExceptionTest = ExceptionTest<AggregateRootIdNotFoundException, AggregateRootIdNotFoundExceptionTest.DefaultExceptionProperties>;

	public class AggregateRootIdNotFoundExceptionTest: ExceptionTest
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public new class ExceptionProperties: ExceptionTest.ExceptionProperties
		{
			public Type AggregateRootType { get; set; }

			public Type AggregateRootIdType { get; set; }
		}

		public class DefaultExceptionProperties: ExceptionProperties
		{
			public DefaultExceptionProperties()
			{
				this.Message = ExceptionMessage.Default<AggregateRootIdNotFoundException>();
			}
		}

		[Fact]
		public void Constructor_CalledWithNullAggregateRootType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new AggregateRootIdNotFoundException(null, DummyType());
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootType");
		}

		private static Type DummyType()
		{
			return Substitute.For<object>().GetType();
		}

		[Fact]
		public void Constructor_CalledWithNullAggregateRootIdType_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new AggregateRootIdNotFoundException(DummyType(), null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootIdType");
		}

		[Fact]
		public void AggregateRootType_Get_ExpectSameValueAsPassedToConstructor()
		{
			var aggregateRootType = DummyType();
			new AggregateRootIdNotFoundException(aggregateRootType, DummyType())
				.AggregateRootType.Should().BeSameAs(aggregateRootType);
		}

		[Fact]
		public void AggregateRootIdType_Get_ExpectSameValueAsPassedToConstructor()
		{
			var aggregateRootIdType = DummyType();
			new AggregateRootIdNotFoundException(DummyType(), aggregateRootIdType)
				.AggregateRootIdType.Should().BeSameAs(aggregateRootIdType);
		}

		[Fact]
		public void Message_Get_ExpectAggregateRootTypeAndIdTypeAreAllIncluded()
		{
			var aggregateRootType = DummyType();
			var aggregateRootIdType = DummyType();
			new AggregateRootIdNotFoundException(aggregateRootType, aggregateRootIdType).Message
				.Should().Contain(aggregateRootType.FullName)
				.And.Subject.Should().Contain(aggregateRootIdType.FullName);
		}
	}
}
