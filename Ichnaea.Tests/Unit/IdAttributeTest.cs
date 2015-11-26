using System;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class IdAttributeTest
	{
		[Fact]
		public void ExpectAttributeIsOnlyValidOnFieldsAndProperties()
		{
			AttributeUsage.ValidOn.Should().HaveFlag(AttributeTargets.Field | AttributeTargets.Property);
		}

		private static AttributeUsageAttribute AttributeUsage => typeof(IdAttribute).GetCustomAttribute<AttributeUsageAttribute>();

		[Fact]
		public void ExpectAttributeIsNotValidOnAnythingOtherThanFieldsAndProperties()
		{
			AttributeUsage.ValidOn.Should().NotHaveFlag(AttributeTargets.All & ~(AttributeTargets.Field | AttributeTargets.Property));
		}

		[Fact]
		public void ExpectAttributeIsOnlyValidOncePerMember()
		{
			AttributeUsage.AllowMultiple.Should().BeFalse();
		}
	}
}
