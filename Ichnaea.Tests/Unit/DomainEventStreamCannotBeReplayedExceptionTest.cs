using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class DomainEventStreamCannotBeReplayedExceptionTest
	{
		private class ExceptionProperties
		{
			public string Message { get; set; }

			public Exception InnerException { get; set; }

			public object AggregateRootId { get; set; }
		}

		private class DefaultExceptionProperties: ExceptionProperties
		{
			public DefaultExceptionProperties()
			{
				this.Message = "Exception of type '" + typeof(DomainEventStreamCannotBeReplayedException) + "' was thrown.";
			}
		}

		private class NonSerialisableTypeWithRandomToString
		{
			private readonly string toString = StringGenerator.AnyNonNull();

			public override string ToString()
			{
				return this.toString;
			}
		}

		[Fact]
		public void Constructor_CalledAsDefaultOverload_ExpectDefaultPropertyValues()
		{
			new DomainEventStreamCannotBeReplayedException().ShouldBeEquivalentTo(
				new DefaultExceptionProperties(),
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithMessage_ExpectSameMessageWithOtherPropertyValuesAsDefaults()
		{
			var expectedProperties = new DefaultExceptionProperties
				{
					Message = StringGenerator.AnyNonNull()
				};

			new DomainEventStreamCannotBeReplayedException(expectedProperties.Message).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithNullMessage_ExpectDefaultPropertyValues()
		{
			new DomainEventStreamCannotBeReplayedException(null).ShouldBeEquivalentTo(
				new DefaultExceptionProperties(),
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithMessageAndNullInnerException_ExpectSameMessageAndInnerExceptionWithOtherPropertyValuesAsDefaults()
		{
			var expectedProperties = new DefaultExceptionProperties
				{
					Message = StringGenerator.AnyNonNull(),
					InnerException = null
				};

			new DomainEventStreamCannotBeReplayedException(expectedProperties.Message, null).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithNullMessageAndNullInnerException_ExpectDefaultPropertyValues()
		{
			new DomainEventStreamCannotBeReplayedException(null, (Exception) null).ShouldBeEquivalentTo(
				new DefaultExceptionProperties(),
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithMessageAndInnerException_ExpectSameMessageAndInnerExceptionWithOtherPropertyValuesAsDefaults()
		{
			var expectedProperties = new DefaultExceptionProperties
				{
					Message = StringGenerator.AnyNonNull(),
					InnerException = new Exception()
				};

			new DomainEventStreamCannotBeReplayedException(expectedProperties.Message, expectedProperties.InnerException).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
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

		private static T SerialiseAndDeserialise<T>(T exception)
		{
			using (var stream = new MemoryStream())
			{
				var serialiser = new BinaryFormatter();
				serialiser.Serialize(stream, exception);

				stream.Position = 0;
				return (T) serialiser.Deserialize(stream);
			}
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
