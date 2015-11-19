using System;
using FluentAssertions;
using Restall.Ichnaea.Tests.Unit.Stubs;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	using ExceptionTest = ExceptionTest<DomainEventStreamCannotBeReplayedException, DomainEventStreamCannotBeReplayedExceptionTest.DefaultExceptionProperties>;

	public class DomainEventStreamCannotBeReplayedExceptionTest: ExceptionTest
	{
		public new class ExceptionProperties: ExceptionTest.ExceptionProperties
		{
			public object AggregateRootId { get; set; }
		}

		public class DefaultExceptionProperties: ExceptionProperties
		{
			public DefaultExceptionProperties()
			{
				this.Message = ExceptionMessage.Default<DomainEventStreamCannotBeReplayedException>();
			}
		}

		[Fact]
		public void Constructor_CalledWithNullMessageAndNullAggregateRootId_ExpectDefaultPropertyValues()
		{
			new DomainEventStreamCannotBeReplayedException(null, (object) null).ShouldBeEquivalentTo(
				new DefaultExceptionProperties(),
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithMessageAndAggregateRootId_ExpectSameMessageAndAggregateRootIdWithOtherPropertyValuesAsDefaults()
		{
			var expectedProperties = new DefaultExceptionProperties
				{
					Message = StringGenerator.AnyNonNull(),
					AggregateRootId = new object()
				};

			new DomainEventStreamCannotBeReplayedException(expectedProperties.Message, expectedProperties.AggregateRootId).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void ExpectSerialisableAggregateRootIdCanBeSerialised()
		{
			var aggregateRootId = Guid.NewGuid();
			var deserialised = SerialiseAndDeserialise(new DomainEventStreamCannotBeReplayedException(null, aggregateRootId));
			deserialised.AggregateRootId.Should().Be(aggregateRootId);
		}

		[Fact]
		public void ExpectNonSerialisableAggregateRootIdIsSerialisedAsToStringValue()
		{
			var aggregateRootId = new NonSerialisableTypeWithRandomToString();
			var deserialised = SerialiseAndDeserialise(new DomainEventStreamCannotBeReplayedException(null, aggregateRootId));
			deserialised.AggregateRootId.Should().Be(aggregateRootId.ToString());
		}
	}
}
