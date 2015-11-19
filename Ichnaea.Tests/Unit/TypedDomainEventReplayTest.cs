using System;
using FluentAssertions;
using NSubstitute;
using Restall.Ichnaea.Tests.Unit.Stubs;
using Xunit;
using Xunit.Extensions;

namespace Restall.Ichnaea.Tests.Unit
{
	public class TypedDomainEventReplayTest
	{
		public class DomainEvent
		{
		}
		public class DerivedDomainEvent: DomainEvent
		{
		}

		public class ConcreteTypedDomainEventReplay: TypedDomainEventReplay<AggregateRootWithPrivateId, DomainEvent>
		{
			public ConcreteTypedDomainEventReplay()
			{
			}

			public ConcreteTypedDomainEventReplay(bool isAggregateRootRequired): base(isAggregateRootRequired)
			{
			}

			protected override AggregateRootWithPrivateId Replay(AggregateRootWithPrivateId aggregateRoot, DomainEvent domainEvent)
			{
				return StronglyTypedReplay(aggregateRoot, domainEvent);
			}

			public virtual AggregateRootWithPrivateId StronglyTypedReplay(AggregateRootWithPrivateId aggregateRoot, DomainEvent domainEvent)
			{
				return null;
			}
		}

		[Fact]
		public void CanReplay_CalledWithNullAggregateRootWhenAggregateRootIsRequired_ExpectFalseIsReturned()
		{
			var replay = new ConcreteTypedDomainEventReplay();
			replay.CanReplay(null, new DomainEvent()).Should().BeFalse();
		}

		[Fact]
		public void CanReplay_CalledWithNullAggregateRootWhenAggregateRootIsNotRequired_ExpectTrueIsReturned()
		{
			var replay = new ConcreteTypedDomainEventReplay(false);
			replay.CanReplay(null, new DomainEvent()).Should().BeTrue();
		}

		[Theory]
		[InlineData(true), InlineData(false)]
		public void CanReplay_CalledWithNullDomainEvent_ExpectFalseIsReturned(bool isAggregateRootRequired)
		{
			var replay = new ConcreteTypedDomainEventReplay(isAggregateRootRequired);
			replay.CanReplay(DummyAggregateRoot(), null).Should().BeFalse();
		}

		private static AggregateRootWithPrivateId DummyAggregateRoot()
		{
			return new AggregateRootWithPrivateId(StringGenerator.AnyNonNull());
		}

		[Theory]
		[InlineData(true), InlineData(false)]
		public void CanReplay_CalledWithAggregateRootAndDomainEventOfUnhandledType_ExpectFalseIsReturned(bool isAggregateRootRequired)
		{
			var replay = new ConcreteTypedDomainEventReplay(isAggregateRootRequired);
			replay.CanReplay(DummyAggregateRoot(), new object()).Should().BeFalse();
		}

		[Theory]
		[InlineData(true), InlineData(false)]
		public void CanReplay_CalledWithNullAggregateRootAndDomainEventOfUnhandledType_ExpectFalseIsReturned(bool isAggregateRootRequired)
		{
			var replay = new ConcreteTypedDomainEventReplay(isAggregateRootRequired);
			replay.CanReplay(null, new object()).Should().BeFalse();
		}

		[Fact]
		public void Replay_CalledWithNullAggregateRootWhenAggregateRootIsRequired_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var replay = new ConcreteTypedDomainEventReplay();
			replay.Invoking(x => x.Replay(null, new DomainEvent())).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
		}

		[Fact]
		public void Replay_CalledWithNullAggregateRootWhenAggregateRootIsNotRequired_ExpectAggregateRootFromStronglyTypedMemberIsReturned()
		{
			ReplayCalledWithNullAggregateRootWhenAggregateRootIsNotRequiredExpectAggregateRootFromStronglyTypedMemberIsReturned(new DomainEvent());
		}

		private static void ReplayCalledWithNullAggregateRootWhenAggregateRootIsNotRequiredExpectAggregateRootFromStronglyTypedMemberIsReturned(DomainEvent domainEvent)
		{
			var aggregateRoot = DummyAggregateRoot();
			var replay = Substitute.ForPartsOf<ConcreteTypedDomainEventReplay>(false);
			replay.StronglyTypedReplay(null, domainEvent).Returns(aggregateRoot);
			replay.Replay(null, domainEvent).Should().BeSameAs(aggregateRoot);
		}

		[Fact]
		public void Replay_CalledWithNullAggregateRootAndDomainEventIsDerivedTypeWhenAggregateRootIsNotRequired_ExpectAggregateRootFromStronglyTypedMemberIsReturned()
		{
			ReplayCalledWithNullAggregateRootWhenAggregateRootIsNotRequiredExpectAggregateRootFromStronglyTypedMemberIsReturned(new DerivedDomainEvent());
		}

		[Theory]
		[InlineData(true), InlineData(false)]
		public void Replay_Called_ExpectAggregateRootFromStronglyTypedMemberIsReturned(bool isAggregateRootRequired)
		{
			var domainEvent = new DomainEvent();
			ReplayCalledExpectAggregateRootFromStronglyTypedMemberIsReturned(isAggregateRootRequired, domainEvent);
		}

		private static void ReplayCalledExpectAggregateRootFromStronglyTypedMemberIsReturned(bool isAggregateRootRequired, DomainEvent domainEvent)
		{
			var aggregateRoot = DummyAggregateRoot();
			var returnedAggregateRoot = DummyAggregateRoot();
			var replay = Substitute.ForPartsOf<ConcreteTypedDomainEventReplay>(isAggregateRootRequired);
			replay.StronglyTypedReplay(aggregateRoot, domainEvent).Returns(returnedAggregateRoot);
			replay.Replay(aggregateRoot, domainEvent).Should().BeSameAs(returnedAggregateRoot);
		}

		[Theory]
		[InlineData(true), InlineData(false)]
		public void Replay_CalledWhenDomainEventIsDerivedType_ExpectAggregateRootFromStronglyTypedMemberIsReturned(bool isAggregateRootRequired)
		{
			var domainEvent = new DerivedDomainEvent();
			ReplayCalledExpectAggregateRootFromStronglyTypedMemberIsReturned(isAggregateRootRequired, domainEvent);
		}

		[Fact]
		public void Replay_CalledWithNullDomainEvent_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var replay = new ConcreteTypedDomainEventReplay();
			replay.Invoking(x => x.Replay(DummyAggregateRoot(), null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("domainEvent");
		}

		[Fact]
		public void Replay_CalledWithDomainEventOfUnhandledType_ExpectDomainEventCannotBeReplayedExceptionWithCorrectDomainEvent()
		{
			var domainEvent = new object();
			var replay = new ConcreteTypedDomainEventReplay();
			replay.Invoking(x => x.Replay(DummyAggregateRoot(), domainEvent))
				.ShouldThrow<DomainEventCannotBeReplayedException>()
				.And.DomainEvent.Should().BeSameAs(domainEvent);
		}

		[Fact]
		public void ExpectCanReplayMethodIsVirtualAndNotFinal()
		{
			var method = Info.OfMethod<TypedDomainEventReplay<object, object>>(x => x.CanReplay(null, null));
			method.IsVirtual.Should().BeTrue();
			method.IsFinal.Should().BeFalse();
		}

		[Fact]
		public void ExpectReplayMethodIsVirtualAndNotFinal()
		{
			var method = Info.OfMethod<TypedDomainEventReplay<object, object>>(x => x.Replay(null, null));
			method.IsVirtual.Should().BeTrue();
			method.IsFinal.Should().BeFalse();
		}
	}
}
