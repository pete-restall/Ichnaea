using System;
using FluentAssertions;
using Restall.Ichnaea.Tests.Unit.Stubs;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	using ExceptionTest = ExceptionTest<DomainEventCannotBeReplayedException, DomainEventCannotBeReplayedExceptionTest.DefaultExceptionProperties>;

	public class DomainEventCannotBeReplayedExceptionTest: ExceptionTest
	{
		public new class ExceptionProperties: ExceptionTest.ExceptionProperties
		{
			public object DomainEvent { get; set; }
		}

		public class DefaultExceptionProperties: ExceptionProperties
		{
			public DefaultExceptionProperties()
			{
				this.Message = ExceptionMessage.Default<DomainEventCannotBeReplayedException>();
			}
		}

		[Fact]
		public void Constructor_CalledWithNullMessageAndNullNullDomainEvent_ExpectDefaultPropertyValues()
		{
			new DomainEventCannotBeReplayedException(null, (object) null).ShouldBeEquivalentTo(
				new DefaultExceptionProperties(),
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithMessageAndDomainEvent_ExpectSameMessageAndDomainEventWithOtherPropertyValuesAsDefaults()
		{
			var expectedProperties = new DefaultExceptionProperties
				{
					Message = StringGenerator.AnyNonNull(),
					DomainEvent = new object()
				};

			new DomainEventCannotBeReplayedException(expectedProperties.Message, expectedProperties.DomainEvent).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void ExpectSerialisableDomainEventCanBeSerialised()
		{
			var domainEvent = Guid.NewGuid();
			var deserialised = SerialiseAndDeserialise(new DomainEventCannotBeReplayedException(null, domainEvent));
			deserialised.DomainEvent.Should().Be(domainEvent);
		}

		[Fact]
		public void ExpectNonSerialisableDomainEventIsSerialisedAsToStringValue()
		{
			var domainEvent = new NonSerialisableTypeWithRandomToString();
			var deserialised = SerialiseAndDeserialise(new DomainEventCannotBeReplayedException(null, domainEvent));
			deserialised.DomainEvent.Should().Be(domainEvent.ToString());
		}
	}
}
