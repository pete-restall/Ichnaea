using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Xunit.Extensions;

namespace Restall.Ichnaea.Tests.Unit
{
	public class DomainEventReplayChainTest
	{
		public enum AggregateRoot
		{
			IsNotNull,
			IsNull
		}

		public enum DomainEvent
		{
			IsNotNull,
			IsNull
		}

		public enum ConstructWith
		{
			Enumerable,
			Params
		}

		private class AllCombinationsOfInlineDataForReplayAttribute: DataAttribute
		{
			public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest, Type[] parameterTypes)
			{
				yield return new object[] {AggregateRoot.IsNotNull, DomainEvent.IsNotNull, ConstructWith.Enumerable};
                yield return new object[] {AggregateRoot.IsNotNull, DomainEvent.IsNotNull, ConstructWith.Params};
				yield return new object[] {AggregateRoot.IsNull, DomainEvent.IsNotNull, ConstructWith.Enumerable};
				yield return new object[] {AggregateRoot.IsNull, DomainEvent.IsNotNull, ConstructWith.Params};
				yield return new object[] {AggregateRoot.IsNotNull, DomainEvent.IsNull, ConstructWith.Enumerable};
				yield return new object[] {AggregateRoot.IsNotNull, DomainEvent.IsNull, ConstructWith.Params};
				yield return new object[] {AggregateRoot.IsNull, DomainEvent.IsNull, ConstructWith.Enumerable};
				yield return new object[] {AggregateRoot.IsNull, DomainEvent.IsNull, ConstructWith.Params};
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullEnumerableReplayChain_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventReplayChain<object>((IEnumerable<IReplayDomainEvents<object>>) null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("replayChain");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		[SuppressMessage("ReSharper", "RedundantCast", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullParamsReplayChain_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventReplayChain<object>(null as IReplayDomainEvents<object>[]);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("replayChain");
		}

		[Theory]
		[AllCombinationsOfInlineDataForReplay]
		public void CanReplay_CalledWhenAnyPartOfChainCanReplay_ExpectTrueIsReturned(
			AggregateRoot aggregateRootOrNull,
			DomainEvent domainEventOrNull,
			ConstructWith construct)
		{
			var aggregateRoot = aggregateRootOrNull.ToObject();
			var domainEvent = domainEventOrNull.ToObject();
			var innerReplays =
				AtLeastOneReplayStubbedForCanReplay(aggregateRoot, domainEvent)
				.Concat(AnyNumberOfReplaysStubbedForCannotReplay())
				.Shuffle()
				.ToArray();

			var chain = construct.WithChain(innerReplays);
			chain.CanReplay(aggregateRoot, domainEvent).Should().BeTrue();
		}

		private static IEnumerable<IReplayDomainEvents<object>> AtLeastOneReplayStubbedForCanReplay(object aggregateRoot, object domainEvent)
		{
			int numberOfReplays = IntegerGenerator.WithinExclusiveRange(1, 10);
			return numberOfReplays.Select(() => CreateReplayStubbedForCanReplay(aggregateRoot, domainEvent, true));
		}

		private static IReplayDomainEvents<object> CreateReplayStubbedForCanReplay(object aggregateRoot, object domainEvent, bool canReplay)
		{
			var replay = Substitute.For<IReplayDomainEvents<object>>();
			replay.CanReplay(aggregateRoot, domainEvent).Returns(canReplay);
			return replay;
		}

		private static IEnumerable<IReplayDomainEvents<object>> AnyNumberOfReplaysStubbedForCannotReplay()
		{
			int numberOfReplays = IntegerGenerator.WithinExclusiveRange(0, 10);
			return numberOfReplays.Select(() => CreateReplayStubbedForCanReplay(Arg.Any<object>(), Arg.Any<object>(), false));
		}

		[Theory]
		[AllCombinationsOfInlineDataForReplay]
		public void CanReplay_CalledWhenChainIsEmpty_ExpectFalseIsReturned(
			AggregateRoot aggregateRootOrNull,
			DomainEvent domainEventOrNull,
			ConstructWith construct)
		{
			var aggregateRoot = aggregateRootOrNull.ToObject();
			var domainEvent = domainEventOrNull.ToObject();
			var chain = construct.WithChain(new IReplayDomainEvents<object>[0]);
			chain.CanReplay(aggregateRoot, domainEvent).Should().BeFalse();
		}

		[Theory]
		[AllCombinationsOfInlineDataForReplay]
		public void CanReplay_CalledWhenAllPartsOfChainCannotReplay_ExpectFalseIsReturned(
			AggregateRoot aggregateRootOrNull,
			DomainEvent domainEventOrNull,
			ConstructWith construct)
		{
			var aggregateRoot = aggregateRootOrNull.ToObject();
			var domainEvent = domainEventOrNull.ToObject();
			var innerReplays = AtLeastOneReplayStubbedForCannotReplay().ToArray();
			var chain = construct.WithChain(innerReplays);
			chain.CanReplay(aggregateRoot, domainEvent).Should().BeFalse();
		}

		private static IEnumerable<IReplayDomainEvents<object>> AtLeastOneReplayStubbedForCannotReplay()
		{
			int numberOfReplays = IntegerGenerator.WithinExclusiveRange(1, 10);
			return numberOfReplays.Select(() => CreateReplayStubbedForCanReplay(Arg.Any<object>(), Arg.Any<object>(), false));
		}

		[Fact]
		public void CanReplay_CalledMultipleTimes_ExpectChainIsOnlyEnumeratedOnce()
		{
			var innerReplays = EnumerableTestDoubles.Mock<IReplayDomainEvents<object>>();
			var chain = new DomainEventReplayChain<object>(innerReplays);
			chain.CanReplay(new object(), new object());
			chain.CanReplay(new object(), new object());
			innerReplays.Received(1).GetEnumerator();
		}

		[Theory]
		[AllCombinationsOfInlineDataForReplay]
		public void Replay_CalledWhenChainIsEmpty_ExpectDomainEventCannotBeReplayedExceptionWithCorrectDomainEvent(
			AggregateRoot aggregateRootOrNull,
			DomainEvent domainEventOrNull,
			ConstructWith construct)
		{
			var aggregateRoot = aggregateRootOrNull.ToObject();
			var domainEvent = domainEventOrNull.ToObject();
			var chain = construct.WithChain(new IReplayDomainEvents<object>[0]);
			chain.Invoking(x => x.Replay(aggregateRoot, domainEvent))
				.ShouldThrow<DomainEventCannotBeReplayedException>()
				.And.DomainEvent.Should().BeSameAs(domainEvent);
		}

		[Theory]
		[AllCombinationsOfInlineDataForReplay]
		public void Replay_CalledWhenOnePartOfChainCanReplay_ExpectAggregateRootReturnedByMatchingChainEntryIsReturned(
			AggregateRoot aggregateRootOrNull,
			DomainEvent domainEventOrNull,
			ConstructWith construct)
		{
			var aggregateRoot = aggregateRootOrNull.ToObject();
			var domainEvent = domainEventOrNull.ToObject();
			var returnedAggregateRoot = new object();
			var innerReplays =
				new[] {CreateReplayStubbedForCanReplay(aggregateRoot, domainEvent, returnedAggregateRoot)}
				.Concat(AnyNumberOfReplaysStubbedForCannotReplay())
				.Shuffle()
				.ToArray();

			var chain = construct.WithChain(innerReplays);
			chain.Replay(aggregateRoot, domainEvent).Should().BeSameAs(returnedAggregateRoot);
		}

		private static IReplayDomainEvents<object> CreateReplayStubbedForCanReplay(object aggregateRoot, object domainEvent, object returnedAggregateRoot)
		{
			var replay = CreateReplayStubbedForCanReplay(aggregateRoot, domainEvent, true);
			replay.Replay(aggregateRoot, domainEvent).Returns(returnedAggregateRoot);
			return replay;
		}

		[Fact]
		public void Replay_CalledWhenChainReturnsNullAggregateRoot_ExpectNullAggregateRootIsReturned()
		{
			var innerReplay = CreateReplayStubbedForCanReplay(Arg.Any<object>(), Arg.Any<object>(), null);
			var chain = new DomainEventReplayChain<object>(innerReplay);
			chain.Replay(new object(), new object()).Should().BeNull();
		}

		[Fact]
		public void Replay_CalledWhenNoPartOfChainCanReplay_ExpectDomainEventCannotBeReplayedExceptionWithCorrectDomainEvent()
		{
			var domainEvent = new object();
			var innerReplay = CreateReplayStubbedForCanReplay(Arg.Any<object>(), Arg.Any<object>(), false);
			var chain = new DomainEventReplayChain<object>(innerReplay);
			chain.Invoking(x => x.Replay(new object(), domainEvent))
				.ShouldThrow<DomainEventCannotBeReplayedException>()
				.And.DomainEvent.Should().BeSameAs(domainEvent);
		}

		[Fact]
		public void Replay_CalledWhenMultiplePartsOfChainCanReplay_ExpectDomainEventCannotBeReplayedExceptionWithCorrectDomainEvent()
		{
			var domainEvent = new object();
			var innerReplay = 2.Select(() => CreateReplayStubbedForCanReplay(Arg.Any<object>(), Arg.Any<object>(), true));
			var chain = new DomainEventReplayChain<object>(innerReplay);
			chain.Invoking(x => x.Replay(new object(), domainEvent))
				.ShouldThrow<DomainEventCannotBeReplayedException>()
				.And.DomainEvent.Should().BeSameAs(domainEvent);
		}

		[Fact]
		public void Replay_CalledMultipleTimes_ExpectChainIsOnlyEnumeratedOnce()
		{
			var innerReplays = EnumerableTestDoubles.Mock(CreateReplayStubbedForCanReplay(Arg.Any<object>(), Arg.Any<object>(), true));
			var chain = new DomainEventReplayChain<object>(innerReplays);
			chain.Replay(new object(), new object());
			chain.Replay(new object(), new object());
			innerReplays.Received(1).GetEnumerator();
		}
	}

	public static class DomainEventReplayChainTestTheoryInlineDataEnumExtensions
	{
		public static object ToObject(this DomainEventReplayChainTest.AggregateRoot aggregateRoot)
		{
			return aggregateRoot == DomainEventReplayChainTest.AggregateRoot.IsNotNull? new object(): null;
		}

		public static object ToObject(this DomainEventReplayChainTest.DomainEvent domainEvent)
		{
			return domainEvent == DomainEventReplayChainTest.DomainEvent.IsNotNull? new object(): null;
		}

		public static DomainEventReplayChain<object> WithChain(this DomainEventReplayChainTest.ConstructWith construct, IReplayDomainEvents<object>[] chain)
		{
			return construct == DomainEventReplayChainTest.ConstructWith.Enumerable
				? new DomainEventReplayChain<object>((IEnumerable<IReplayDomainEvents<object>>) chain)
				: new DomainEventReplayChain<object>(chain);
		}
	}
}
