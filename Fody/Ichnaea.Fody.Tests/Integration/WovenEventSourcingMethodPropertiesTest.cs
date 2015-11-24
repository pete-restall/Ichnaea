using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class WovenEventSourcingMethodPropertiesTest
	{
		private readonly MethodInfo eventSourcingMethod;

		public WovenEventSourcingMethodPropertiesTest()
		{
			var aggregateRootType = ModuleWeaverFixture.AggregateRootFactory.CreateAggregateRootFromClassNamed("OneSourceEventCall").GetType();
			this.eventSourcingMethod = AllMethodsFrom(aggregateRootType).Single(x => x.Name.StartsWith("<Ichnaea>SourceEvent"));
		}

		private static IEnumerable<MethodInfo> AllMethodsFrom(IReflect type)
		{
			return type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		[Fact]
		public void ExpectEventSourcingMethodIsPrivate()
		{
			this.eventSourcingMethod.Attributes.Should().HaveFlag(MethodAttributes.Private);
		}

		[Fact]
		public void ExpectEventSourcingMethodIsNotVirtual()
		{
			this.eventSourcingMethod.Attributes.Should().NotHaveFlag(MethodAttributes.Virtual);
		}

		[Fact]
		public void ExpectEventSourcingMethodIsSpecialName()
		{
			this.eventSourcingMethod.Attributes.Should().HaveFlag(MethodAttributes.SpecialName);
		}

		[Fact]
		public void ExpectEventSourcingMethodIsHideBySig()
		{
			this.eventSourcingMethod.Attributes.Should().HaveFlag(MethodAttributes.HideBySig);
		}
	}
}
