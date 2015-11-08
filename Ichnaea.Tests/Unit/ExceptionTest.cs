using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public abstract class ExceptionTest<TException, TDefaultExceptionProperties>
		where TException: Exception
		where TDefaultExceptionProperties: ExceptionTest<TException, TDefaultExceptionProperties>.ExceptionProperties, new()
	{
		public class ExceptionProperties
		{
			public string Message { get; set; }

			public Exception InnerException { get; set; }
		}

		[Fact]
		public void Constructor_CalledAsDefaultOverload_ExpectDefaultPropertyValues()
		{
			Constructor().ShouldBeEquivalentTo(
				new TDefaultExceptionProperties(),
				options => options.ExcludingMissingMembers());
		}

		private static TException Constructor()
		{
			return Activator.CreateInstance<TException>();
		}

		[Fact]
		public void Constructor_CalledWithMessage_ExpectSameMessageWithOtherPropertyValuesAsDefaults()
		{
			var expectedProperties = new TDefaultExceptionProperties
				{
					Message = StringGenerator.AnyNonNull()
				};

			Constructor(expectedProperties.Message).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
		}

		private static TException Constructor(string message)
		{
			return (TException) Activator.CreateInstance(typeof(TException), message);
		}

		[Fact]
		public void Constructor_CalledWithNullMessage_ExpectDefaultPropertyValues()
		{
			Constructor(null).ShouldBeEquivalentTo(
				new TDefaultExceptionProperties(),
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithMessageAndNullInnerException_ExpectSameMessageAndInnerExceptionWithOtherPropertyValuesAsDefaults()
		{
			var expectedProperties = new TDefaultExceptionProperties
				{
					Message = StringGenerator.AnyNonNull(),
					InnerException = null
				};

			Constructor(expectedProperties.Message, null).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
		}

		private static TException Constructor(string message, Exception innerException)
		{
			return (TException) Activator.CreateInstance(typeof(TException), message, innerException);
		}

		[Fact]
		public void Constructor_CalledWithNullMessageAndNullInnerException_ExpectDefaultPropertyValues()
		{
			Constructor(null, null).ShouldBeEquivalentTo(
				new TDefaultExceptionProperties(),
				options => options.ExcludingMissingMembers());
		}

		[Fact]
		public void Constructor_CalledWithMessageAndInnerException_ExpectSameMessageAndInnerExceptionWithOtherPropertyValuesAsDefaults()
		{
			var expectedProperties = new TDefaultExceptionProperties
				{
					Message = StringGenerator.AnyNonNull(),
					InnerException = new Exception()
				};

			Constructor(expectedProperties.Message, expectedProperties.InnerException).ShouldBeEquivalentTo(
				expectedProperties,
				options => options.ExcludingMissingMembers());
		}
	}
}
