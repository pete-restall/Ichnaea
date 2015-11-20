using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Restall.Ichnaea.Tests.Unit.Stubs;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class TypedPropertyAggregateRootIdGetterTest
	{
		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
		private class AggregateRoot
		{
			public static decimal StaticDecimalProperty { get; set; }

			[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public object ObjectProperty { get; set; }

			public string StringProperty { get; set; }
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
		private class AggregateRootWithSameIdTypesAndVisibility
		{
			public decimal DecimalProperty1 { get; set; }

			public decimal DecimalProperty2 { get; set; }
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
		private class AggregateRootWithSameIdTypesAndDifferentVisibility
		{
			public Guid PublicGuidProperty { get; set; }

			private Guid PrivateGuidProperty { get; set; }
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenAggregateRootTypeDoesNotHavePropertyWithIdType_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new TypedPropertyAggregateRootIdGetter<AggregateRoot, int>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<AggregateRoot, int>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenPropertyIsStatic_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new TypedPropertyAggregateRootIdGetter<AggregateRoot, decimal>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<AggregateRoot, decimal>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenMultiplePropertiesOfCorrectTypeAndSameVisibility_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new TypedPropertyAggregateRootIdGetter<AggregateRootWithSameIdTypesAndVisibility, decimal>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<AggregateRootWithSameIdTypesAndVisibility, decimal>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenMultiplePropertiesOfCorrectTypeAndDifferentVisibility_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new TypedPropertyAggregateRootIdGetter<AggregateRootWithSameIdTypesAndDifferentVisibility, Guid>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<AggregateRootWithSameIdTypesAndDifferentVisibility, Guid>();
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
