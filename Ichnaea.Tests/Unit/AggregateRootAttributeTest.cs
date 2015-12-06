using System;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class AggregateRootAttributeTest
	{
		[Fact]
		public void ExpectAttributeIsOnlyValidOnClasses()
		{
			AttributeUsage.ValidOn.Should().HaveFlag(AttributeTargets.Class);
		}

		private static AttributeUsageAttribute AttributeUsage => typeof(AggregateRootAttribute).GetCustomAttribute<AttributeUsageAttribute>();

		[Fact]
		public void ExpectAttributeIsNotValidOnAnythingOtherThanClasses()
		{
			AttributeUsage.ValidOn.Should().NotHaveFlag(AttributeTargets.All & ~AttributeTargets.Class);
		}

		[Fact]
		public void ExpectAttributeIsOnlyValidOncePerClass()
		{
			AttributeUsage.AllowMultiple.Should().BeFalse();
		}
	}
}
