using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class IdAttributeAggregateRootIdGetterTest
	{
		private class AggregateRootWithoutDecoratedMembers
		{
#pragma warning disable 169
			private readonly int id;
#pragma warning restore 169

			[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty", Justification = CodeAnalysisJustification.StubForTesting)]
			[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public int Id { get; }
		}

		private class AggregateRootWithIdField<T>
		{
			[Id]
			[SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			private readonly T id;

			public AggregateRootWithIdField(T id)
			{
				this.id = id;
			}
		}

		private class AggregateRootWithMultipleDecoratedFields
		{
#pragma warning disable 169
			[Id]
			private readonly int id1;

			[Id]
			private readonly int id2;
#pragma warning restore 169
		}

		private class DerivedAggregateRootWithMultipleDecoratedFields: AggregateRootWithIdField<object>
		{
#pragma warning disable 169
			[Id]
			private readonly int anotherId;
#pragma warning restore 169

			public DerivedAggregateRootWithMultipleDecoratedFields(): base(new object())
			{
			}
		}

		private class AggregateRootWithIdProperty<T>
		{
			[Id]
			[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			private T Id { get; }

			public AggregateRootWithIdProperty(T id)
			{
				this.Id = id;
			}
		}

		[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty", Justification = CodeAnalysisJustification.StubForTesting)]
		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
		private class AggregateRootWithMultipleDecoratedProperties
		{
			[Id]
			public int Id1 { get; }

			[Id]
			public int Id2 { get; }
		}

		private class DerivedAggregateRootWithMultipleDecoratedProperties: AggregateRootWithIdProperty<object>
		{
			[Id]
			[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty", Justification = CodeAnalysisJustification.StubForTesting)]
			private int AnotherId { get; }

			public DerivedAggregateRootWithMultipleDecoratedProperties(): base(new object())
			{
			}
		}

		private class AggregateRootWithDecoratedFieldAndProperty
		{
#pragma warning disable 169
			[Id]
			private readonly int id1;
#pragma warning restore 169

			[Id]
			[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty", Justification = CodeAnalysisJustification.StubForTesting)]
			[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public int Id2 { get; }
		}

		private class DerivedAggregateRootWithDecoratedFieldAndProperty: AggregateRootWithIdField<object>
		{
			public DerivedAggregateRootWithDecoratedFieldAndProperty(): base(new object())
			{
			}

			[Id]
			[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty", Justification = CodeAnalysisJustification.StubForTesting)]
			[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public int AnotherId { get; }
		}

		private class AggregateRootWithDecoratedStaticField
		{
#pragma warning disable 169
			private static readonly int id;
#pragma warning restore 169
		}

		private class AggregateRootWithDecoratedStaticProperty
		{
			[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty", Justification = CodeAnalysisJustification.StubForTesting)]
			[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public static int Id { get; }
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenAggregateRootTypeDoesNotHaveDecoratedFieldOrProperty_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new IdAttributeAggregateRootIdGetter<AggregateRootWithoutDecoratedMembers, int>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<AggregateRootWithoutDecoratedMembers, int>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenAggregateRootTypeHasMultipleDecoratedFields_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new IdAttributeAggregateRootIdGetter<AggregateRootWithMultipleDecoratedFields, int>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<AggregateRootWithMultipleDecoratedFields, int>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenDerivedAggregateRootTypeHasMultipleDecoratedFields_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new IdAttributeAggregateRootIdGetter<DerivedAggregateRootWithMultipleDecoratedFields, object>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<DerivedAggregateRootWithMultipleDecoratedFields, object>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenAggregateRootTypeHasMultipleDecoratedProperties_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new IdAttributeAggregateRootIdGetter<AggregateRootWithMultipleDecoratedProperties, int>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<AggregateRootWithMultipleDecoratedProperties, int>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenDerivedAggregateRootTypeHasMultipleDecoratedProperties_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new IdAttributeAggregateRootIdGetter<DerivedAggregateRootWithMultipleDecoratedProperties, object>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<DerivedAggregateRootWithMultipleDecoratedProperties, object>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenAggregateRootTypeHasDecoratedFieldAndProperty_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new IdAttributeAggregateRootIdGetter<AggregateRootWithDecoratedFieldAndProperty, int>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<AggregateRootWithDecoratedFieldAndProperty, int>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenDerivedAggregateRootTypeHasDecoratedFieldAndProperty_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new IdAttributeAggregateRootIdGetter<DerivedAggregateRootWithDecoratedFieldAndProperty, object>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<DerivedAggregateRootWithDecoratedFieldAndProperty, object>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenAggregateRootTypeHasDecoratedStaticField_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new IdAttributeAggregateRootIdGetter<AggregateRootWithDecoratedStaticField, int>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<AggregateRootWithDecoratedStaticField, int>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWhenAggregateRootTypeHasDecoratedStaticProperty_ExpectAggregateRootIdNotFoundExceptionWithCorrectTypes()
		{
			Action constructor = () => new IdAttributeAggregateRootIdGetter<AggregateRootWithDecoratedStaticProperty, int>();
			constructor.ShouldThrowAggregateRootIdNotFoundExceptionFor<AggregateRootWithDecoratedStaticProperty, int>();
		}

		[Fact]
		public void GetIdFrom_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var getter = new IdAttributeAggregateRootIdGetter<AggregateRootWithIdField<int>, int>();
			getter.Invoking(x => x.GetIdFrom(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
		}

		[Fact]
		public void GetIdFrom_CalledWithAggregateRootWithIdField_ExpectTheValueOfTheFieldIsReturned()
		{
			var getter = new IdAttributeAggregateRootIdGetter<AggregateRootWithIdField<object>, object>();
			var id = new object();
			getter.GetIdFrom(new AggregateRootWithIdField<object>(id)).Should().BeSameAs(id);
		}

		[Fact]
		public void GetIdFrom_CalledWithAggregateRootWithNullIdField_ExpectInvalidOperationException()
		{
			var getter = new IdAttributeAggregateRootIdGetter<AggregateRootWithIdField<object>, object>();
			getter.Invoking(x => x.GetIdFrom(new AggregateRootWithIdField<object>(null))).ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public void GetIdFrom_CalledWithAggregateRootWithIdProperty_ExpectTheValueOfThePropertyIsReturned()
		{
			var getter = new IdAttributeAggregateRootIdGetter<AggregateRootWithIdProperty<object>, object>();
			var id = new object();
			getter.GetIdFrom(new AggregateRootWithIdProperty<object>(id)).Should().BeSameAs(id);
		}

		[Fact]
		public void GetIdFrom_CalledWithAggregateRootWithNullIdProperty_ExpectInvalidOperationException()
		{
			var getter = new IdAttributeAggregateRootIdGetter<AggregateRootWithIdProperty<object>, object>();
			getter.Invoking(x => x.GetIdFrom(new AggregateRootWithIdProperty<object>(null))).ShouldThrow<InvalidOperationException>();
		}
	}
}
