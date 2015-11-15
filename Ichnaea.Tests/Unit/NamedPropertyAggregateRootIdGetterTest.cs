using System;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class NamedPropertyAggregateRootIdGetterTest
	{
		private class AggregateRoot
		{
			public static object StaticObjectProperty { get; }

			public string StringProperty { get; set; }

			public object ObjectProperty { get; set; }
		}

		private class AggregateRootWithPrivateId
		{
			public const string PropertyName = nameof(Id);

			public AggregateRootWithPrivateId(string id)
			{
				this.Id = id;
			}

			private string Id { get; }
		}

		private class AggregateRootWithProtectedId
		{
			public const string PropertyName = nameof(Id);

			public AggregateRootWithProtectedId(string id)
			{
				this.Id = id;
			}

			protected string Id { get; }
		}

		private class AggregateRootWithProtectedInternalId
		{
			public const string PropertyName = nameof(Id);

			public AggregateRootWithProtectedInternalId(string id)
			{
				this.Id = id;
			}

			protected internal string Id { get; }
		}

		private class AggregateRootWithInternalId
		{
			public const string PropertyName = nameof(Id);

			public AggregateRootWithInternalId(string id)
			{
				this.Id = id;
			}

			internal string Id { get; }
		}

		private class AggregateRootWithShadowedIdBase
		{
			protected AggregateRootWithShadowedIdBase(string id)
			{
				this.Id = id;
			}

			public string Id { get; }
		}

		private class AggregateRootWithShadowedId : AggregateRootWithShadowedIdBase
		{
			public const string PropertyName = nameof(Id);

			public AggregateRootWithShadowedId(string baseId, string derivedId) : base(baseId)
			{
				this.Id = derivedId;
			}

			public new string Id { get; }
		}

		[Fact]
		public void Constructor_CalledWithNullPropertyName_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new NamedPropertyAggregateRootIdGetter<AggregateRoot, object>(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("propertyName");
		}

		[Fact]
		public void Constructor_CalledWhenPropertyDoesNotExist_ExpectAggregateRootIdPropertyNotFoundExceptionWithCorrectPropertyNameAndTypes()
		{
			var propertyName = DummyPropertyName();
			Action constructor = () => new NamedPropertyAggregateRootIdGetter<AggregateRoot, object>(propertyName);
			constructor.ShouldThrowAggregateRootIdPropertyNotFoundExceptionFor<AggregateRoot, object>(propertyName);
		}

		private static string DummyPropertyName()
		{
			return "property" + Guid.NewGuid();
		}

		[Fact]
		public void Constructor_CalledWhenPropertyIsDifferentButCastableType_ExpectAggregateRootIdPropertyNotFoundExceptionWithCorrectPropertyNameAndTypes()
		{
			var propertyName = Info.OfProperty<AggregateRoot>(x => x.StringProperty).Name;
			Action constructor = () => new NamedPropertyAggregateRootIdGetter<AggregateRoot, object>(propertyName);
			constructor.ShouldThrowAggregateRootIdPropertyNotFoundExceptionFor<AggregateRoot, object>(propertyName);
		}

		[Fact]
		public void Constructor_CalledWhenPropertyIsStatic_ExpectAggregateRootIdPropertyNotFoundExceptionWithCorrectPropertyNameAndTypes()
		{
			var propertyName = Info.OfProperty(() => AggregateRoot.StaticObjectProperty).Name;
			Action constructor = () => new NamedPropertyAggregateRootIdGetter<AggregateRoot, object>(propertyName);
			constructor.ShouldThrowAggregateRootIdPropertyNotFoundExceptionFor<AggregateRoot, object>(propertyName);
		}

		[Fact]
		public void GetIdFrom_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var getter = new NamedPropertyAggregateRootIdGetter<AggregateRoot, object>(Info.OfProperty<AggregateRoot>(x => x.ObjectProperty).Name);
			getter.Invoking(x => x.GetIdFrom(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsNull_ExpectInvalidOperationException()
		{
			var propertyName = Info.OfProperty<AggregateRoot>(x => x.ObjectProperty).Name;
			var getter = new NamedPropertyAggregateRootIdGetter<AggregateRoot, object>(propertyName);
			var aggregateRoot = new AggregateRoot { ObjectProperty = null };
			getter.Invoking(x => x.GetIdFrom(aggregateRoot)).ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsPublic_ExpectValueOfPropertyIsReturned()
		{
			var propertyName = Info.OfProperty<AggregateRoot>(x => x.ObjectProperty).Name;
			var getter = new NamedPropertyAggregateRootIdGetter<AggregateRoot, object>(propertyName);
			var aggregateRoot = new AggregateRoot { ObjectProperty = new object() };
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(aggregateRoot.ObjectProperty);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsPrivate_ExpectValueOfPropertyIsReturned()
		{
			var id = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithPrivateId(id);
			var getter = new NamedPropertyAggregateRootIdGetter<AggregateRootWithPrivateId, string>(AggregateRootWithPrivateId.PropertyName);
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(id);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsProtected_ExpectValueOfPropertyIsReturned()
		{
			var id = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithProtectedId(id);
			var getter = new NamedPropertyAggregateRootIdGetter<AggregateRootWithProtectedId, string>(AggregateRootWithProtectedId.PropertyName);
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(id);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsProtectedInternal_ExpectValueOfPropertyIsReturned()
		{
			var id = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithProtectedInternalId(id);
			var getter = new NamedPropertyAggregateRootIdGetter<AggregateRootWithProtectedInternalId, string>(AggregateRootWithProtectedInternalId.PropertyName);
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(id);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsInternal_ExpectValueOfPropertyIsReturned()
		{
			var id = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithInternalId(id);
			var getter = new NamedPropertyAggregateRootIdGetter<AggregateRootWithInternalId, string>(AggregateRootWithInternalId.PropertyName);
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(id);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsShadowedAndAggregateRootIsBase_ExpectValueOfBasePropertyIsReturned()
		{
			var baseId = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithShadowedId(baseId, StringGenerator.AnyNonNull());
			var getter = new NamedPropertyAggregateRootIdGetter<AggregateRootWithShadowedIdBase, string>(AggregateRootWithShadowedId.PropertyName);
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(baseId);
		}

		[Fact]
		public void GetIdFrom_CalledWhenPropertyIsShadowedAndAggregateRootIsDerived_ExpectValueOfDerivedPropertyIsReturned()
		{
			var derivedId = StringGenerator.AnyNonNull();
			var aggregateRoot = new AggregateRootWithShadowedId(StringGenerator.AnyNonNull(), derivedId);
			var getter = new NamedPropertyAggregateRootIdGetter<AggregateRootWithShadowedId, string>(AggregateRootWithShadowedId.PropertyName);
			getter.GetIdFrom(aggregateRoot).Should().BeSameAs(derivedId);
		}

		// TODO: TryConstruct() METHOD - ALLOWS AN AggregateRootIdGetterChain / Strategy TO BE CREATED TO ALLOW SENSIBLE DEFAULTS (IE. ALLOWS MULTIPLE PROPERTY NAMES TO BE TRIED, ETC.) WITHOUT HAVING TO CONFIGURE ID RESOLUTION MANUALLY FOR EVERY AGGREGATE ROOT
		// TODO: ALSO ADD A TypedPropertyAggregateRootIdGetter CLASS, TOO
		// TODO: ALSO ADD AN IdAttributeAggregateRootIdGetter CLASS, TOO
	}
}
