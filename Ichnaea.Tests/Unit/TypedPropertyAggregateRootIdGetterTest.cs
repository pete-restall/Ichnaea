using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Restall.Ichnaea.Tests.Unit.Stubs;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	[SuppressMessage("ReSharper", "UnusedMember.Local")]
	public class TypedPropertyAggregateRootIdGetterTest
	{
		private class AggregateRoot
		{
			public static decimal StaticDecimalProperty { get; set; }

			public object ObjectProperty { get; set; }

			public string StringProperty { get; set; }
		}

		private class AggregateRootWithSameIdTypesAndVisibility
		{
			public decimal DecimalProperty1 { get; set; }

			public decimal DecimalProperty2 { get; set; }
		}

		private class AggregateRootWithSameIdTypesAndDifferentVisibility
		{
			public Guid PublicGuidProperty { get; set; }

			private Guid PrivateGuidProperty { get; set; }
		}

		[Fact]
		public void Constructor_CalledWhenAggregateRootTypeDoesNotHavePropertyWithIdType_ExpectAggregateRootIdPropertyNotFoundExceptionWithCorrectTypesAndEmptyPropertyName()
		{
			Action constructor = () => new TypedPropertyAggregateRootIdGetter<AggregateRoot, int>();
			constructor.ShouldThrowAggregateRootIdPropertyNotFoundExceptionFor<AggregateRoot, int>(string.Empty);
		}

		[Fact]
		public void Constructor_CalledWhenPropertyIsStatic_ExpectAggregateRootIdPropertyNotFoundExceptionWithCorrectTypesAndEmptyPropertyName()
		{
			Action constructor = () => new TypedPropertyAggregateRootIdGetter<AggregateRoot, decimal>();
			constructor.ShouldThrowAggregateRootIdPropertyNotFoundExceptionFor<AggregateRoot, decimal>(string.Empty);
		}

		[Fact]
		public void Constructor_CalledWhenMultiplePropertiesOfCorrectTypeAndSameVisibility_ExpectAggregateRootIdPropertyNotFoundExceptionWithCorrectTypesAndEmptyPropertyName()
		{
			Action constructor = () => new TypedPropertyAggregateRootIdGetter<AggregateRootWithSameIdTypesAndVisibility, decimal>();
			constructor.ShouldThrowAggregateRootIdPropertyNotFoundExceptionFor<AggregateRootWithSameIdTypesAndVisibility, decimal>(string.Empty);
		}

		[Fact]
		public void Constructor_CalledWhenMultiplePropertiesOfCorrectTypeAndDifferentVisibility_ExpectAggregateRootIdPropertyNotFoundExceptionWithCorrectTypesAndEmptyPropertyName()
		{
			Action constructor = () => new TypedPropertyAggregateRootIdGetter<AggregateRootWithSameIdTypesAndDifferentVisibility, Guid>();
			constructor.ShouldThrowAggregateRootIdPropertyNotFoundExceptionFor<AggregateRootWithSameIdTypesAndDifferentVisibility, Guid>(string.Empty);
		}

		[Fact]
		public void GetIdFrom_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var getter = new TypedPropertyAggregateRootIdGetter<AggregateRoot, object>();
			getter.Invoking(x => x.GetIdFrom(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsNull_ExpectInvalidOperationException()
		{
			var aggregateRoot = new AggregateRoot { ObjectProperty = null };
			var getter = new TypedPropertyAggregateRootIdGetter<AggregateRoot, object>();
			getter.Invoking(x => x.GetIdFrom(aggregateRoot)).ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsPublic_ExpectValueOfPropertyIsReturned()
		{
			var aggregateRoot = new AggregateRoot { StringProperty = StringGenerator.AnyNonNull() };
			var getter = new TypedPropertyAggregateRootIdGetter<AggregateRoot, string>();
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(aggregateRoot.StringProperty);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsPrivate_ExpectValueOfPropertyIsReturned()
		{
			var id = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithPrivateId(id);
			var getter = new TypedPropertyAggregateRootIdGetter<AggregateRootWithPrivateId, string>();
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(id);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsProtected_ExpectValueOfPropertyIsReturned()
		{
			var id = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithProtectedId(id);
			var getter = new TypedPropertyAggregateRootIdGetter<AggregateRootWithProtectedId, string>();
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(id);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsInternal_ExpectValueOfPropertyIsReturned()
		{
			var id = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithInternalId(id);
			var getter = new TypedPropertyAggregateRootIdGetter<AggregateRootWithInternalId, string>();
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(id);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsProtectedInternal_ExpectValueOfPropertyIsReturned()
		{
			var id = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithProtectedInternalId(id);
			var getter = new TypedPropertyAggregateRootIdGetter<AggregateRootWithProtectedInternalId, string>();
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(id);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsShadowedAndAggregateRootIsBase_ExpectValueOfBasePropertyIsReturned()
		{
			var baseId = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithShadowedId(baseId, StringGenerator.AnyNonNull());
			var getter = new TypedPropertyAggregateRootIdGetter<AggregateRootWithShadowedIdBase, string>();
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(baseId);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsShadowedAndAggregateRootIsDerived_ExpectValueOfDerivedPropertyIsReturned()
		{
			var derivedId = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithShadowedId(StringGenerator.AnyNonNull(), derivedId);
			var getter = new TypedPropertyAggregateRootIdGetter<AggregateRootWithShadowedId, string>();
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(derivedId);
		}
	}
}
