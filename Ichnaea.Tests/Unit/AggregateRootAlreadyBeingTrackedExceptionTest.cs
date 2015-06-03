using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class AggregateRootAlreadyBeingTrackedExceptionTest
	{
		private class ExceptionProperties
		{
			public string Message { get; set; }

			public Exception InnerException { get; set; }
		}

		private class DefaultExceptionProperties: ExceptionProperties
		{
			public DefaultExceptionProperties()
			{
				this.Message = "Exception of type '" + typeof(AggregateRootAlreadyBeingTrackedException) + "' was thrown.";
			}
		}

		[Fact]
		public void Constructor_CalledAsDefaultOverload_ExpectDefaultPropertyValues()
		{
			new AggregateRootAlreadyBeingTrackedException().ShouldBeEquivalentTo(
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

			new AggregateRootAlreadyBeingTrackedException(expectedProperties.Message).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithNullMessage_ExpectDefaultPropertyValues()
		{
			new AggregateRootAlreadyBeingTrackedException(null).ShouldBeEquivalentTo(
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

			new AggregateRootAlreadyBeingTrackedException(expectedProperties.Message, null).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithNullMessageAndNullInnerException_ExpectDefaultPropertyValues()
		{
			new AggregateRootAlreadyBeingTrackedException(null, null).ShouldBeEquivalentTo(
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

			new AggregateRootAlreadyBeingTrackedException(expectedProperties.Message, expectedProperties.InnerException).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
		}
	}
}
