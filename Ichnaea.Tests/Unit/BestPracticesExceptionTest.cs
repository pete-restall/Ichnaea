﻿using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class BestPracticesExceptionTest
	{
		[Fact]
		public void ExpectExceptionsAreImplementedAsPerMicrosoftBestPractices()
		{
			AutoTest.Exceptions.ExceptionTester.TestAllExceptions(typeof(DomainEventStreamCannotBeReplayedException).Assembly)
				.Should().OnlyContain(x => x.Success);
		}
	}
}
