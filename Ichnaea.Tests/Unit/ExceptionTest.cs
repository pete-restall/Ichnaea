﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
			var constructor = typeof(TException).GetConstructor(new[] {typeof(string), typeof(Exception)});
			constructor.Should().NotBeNull("because .ctor(string, Exception) is a standard Exception constructor");
			return (TException) constructor?.Invoke(new object[] {message, innerException});
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

		protected static T SerialiseAndDeserialise<T>(T exception) where T: Exception
		{
			using (var stream = new MemoryStream())
			{
				var serialiser = new BinaryFormatter();
				serialiser.Serialize(stream, exception);

				stream.Position = 0;
				return (T) serialiser.Deserialize(stream);
			}
		}
	}
}
