using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NEventStore;
using NSubstitute;
using Restall.Ichnaea.NEventStore;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit.NEventStore
{
	public class DomainEventStreamTest
	{
		[Fact]
		public void Constructor_CalledWithNullEventStore_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				null, DummyPrePersistenceTracker<object>(), DummyAggregateRootIdGetter, DummyBucketId(), DummyEventConverter, DummyEventReplay());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventStore");
		}

		private static IPrePersistenceDomainEventTracker<T> DummyPrePersistenceTracker<T>()
		{
			return Substitute.For<IPrePersistenceDomainEventTracker<T>>();
		}

		private static string DummyAggregateRootIdGetter<T>(T aggregateRoot)
		{
			return string.Empty;
		}

		private static string DummyBucketId()
		{
			return StringGenerator.AnyNonNull();
		}

		private static EventMessage DummyEventConverter(object domainEvent)
		{
			return new EventMessage();
		}

		private static IReplayDomainEvents<object> DummyEventReplay()
		{
			return Substitute.For<IReplayDomainEvents<object>>();
		}

		[Fact]
		public void Constructor_CalledWithNullPrePersistenceDomainEventTracker_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				DummyEventStore(), null, DummyAggregateRootIdGetter, DummyBucketId(), DummyEventConverter, DummyEventReplay());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("prePersistenceDomainEventTracker");
		}

		private static IStoreEvents DummyEventStore()
		{
			return Substitute.For<IStoreEvents>();
		}

		[Fact]
		public void Constructor_CalledWithNullAggregateRootIdGetter_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				DummyEventStore(), DummyPrePersistenceTracker<object>(), null, DummyBucketId(), DummyEventConverter, DummyEventReplay());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootIdGetter");
		}

		[Fact]
		public void Constructor_CalledWithNullBucketId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				DummyEventStore(), DummyPrePersistenceTracker<object>(), DummyAggregateRootIdGetter, null, DummyEventConverter, DummyEventReplay());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("bucketId");
		}

		[Fact]
		public void Constructor_CalledWithNullDomainEventToPersistableConverter_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				DummyEventStore(), DummyPrePersistenceTracker<object>(), DummyAggregateRootIdGetter, DummyBucketId(), null, DummyEventReplay());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("domainEventToPersistableConverter");
		}

		[Fact]
		public void Constructor_CalledWithNullDomainEventReplay_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				DummyEventStore(), DummyPrePersistenceTracker<object>(), DummyAggregateRootIdGetter, DummyBucketId(), DummyEventConverter, null);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("domainEventReplay");
		}

		[Fact]
		public void CreateFrom_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var stream = new DomainEventStream<object>(
				DummyEventStore(),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				stream.Invoking(x => x.CreateFrom(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
			}
		}

		[Fact]
		public void CreateFrom_Called_ExpectEventStoreStreamIsCreatedWithCorrectBucketIdAndIdFromAggregateRoot()
		{
			string bucketId = StringGenerator.AnyNonNull();
			string aggregateRootId = StringGenerator.AnyNonNull();
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.CreateStream(bucketId, aggregateRootId).Returns(Substitute.For<IEventStream>());

			var aggregateRoot = new object();
			using (var stream = new DomainEventStream<object>(
				eventStore,
				DummyPrePersistenceTracker<object>(),
				StubAggregateRootIdGetter(aggregateRoot, aggregateRootId),
				bucketId,
				DummyEventConverter,
				DummyEventReplay()))
			{
				stream.CreateFrom(aggregateRoot);
			}

			eventStore.Received(1).CreateStream(bucketId, aggregateRootId);
		}

		private static AggregateRootIdGetter<object> StubAggregateRootIdGetter(object aggregateRoot, string aggregateRootId)
		{
			var aggregateRootIdGetter = Substitute.For<AggregateRootIdGetter<object>>();
			aggregateRootIdGetter(aggregateRoot).Returns(aggregateRootId);
			return aggregateRootIdGetter;
		}

		[Fact]
		public void CreateFrom_Called_ExpectPrePersistenceTrackerIsSwitchedToTheEventStoreForTheAggregateRoot()
		{
			var prePersistenceTracker = Substitute.For<IPrePersistenceDomainEventTracker<object>>();
			using (var stream = new DomainEventStream<object>(
				DummyEventStore(),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				var aggregateRoot = new object();
				stream.CreateFrom(aggregateRoot);
				prePersistenceTracker.Received(1).SwitchTrackingToPersistentStore(aggregateRoot, Arg.Is<Source.Of<object>>(x => x != null));
			}
		}

		[Fact]
		public void CreateFrom_Called_ExpectDomainEventsFromPrePersistenceTrackerAreWrittenToTheEventStore()
		{
			var domainEvents = CreateAtLeastOneDomainEvent();
			var eventStoreEvents = domainEvents.Select(x => new EventMessage()).ToArray();

			string bucketId = StringGenerator.AnyNonNull();
			string aggregateRootId = StringGenerator.AnyNonNull();
			var eventStoreStream = Substitute.For<IEventStream>();
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.CreateStream(bucketId, aggregateRootId).Returns(eventStoreStream);

			var aggregateRoot = new object();
			using (var stream = new DomainEventStream<object>(
				eventStore,
				StubPrePersistenceDomainEventTracker(aggregateRoot, domainEvents),
				StubAggregateRootIdGetter(aggregateRoot, aggregateRootId),
				bucketId,
				StubEventMessageConverter(domainEvents, eventStoreEvents),
				DummyEventReplay()))
			{
				stream.CreateFrom(aggregateRoot);
			}

			Received.InOrder(() => domainEvents.Length.Repeat(i => eventStoreStream.Add(eventStoreEvents[i])));
		}

		private static object[] CreateAtLeastOneDomainEvent()
		{
			int numberOfDomainEvents = IntegerGenerator.WithinExclusiveRange(1, 10);
			return numberOfDomainEvents.Select(() => new object()).ToArray();
		}

		private static IPrePersistenceDomainEventTracker<object> StubPrePersistenceDomainEventTracker(object aggregateRoot, IEnumerable<object> domainEvents)
		{
			var prePersistenceTracker = Substitute.For<IPrePersistenceDomainEventTracker<object>>();
			prePersistenceTracker.SwitchTrackingToPersistentStore(
				aggregateRoot,
				Arg.Do<Source.Of<object>>(sourceEvent => domainEvents.ForEach(domainEvent => sourceEvent(aggregateRoot, domainEvent))));

			return prePersistenceTracker;
		}

		private static Converter<object, EventMessage> StubEventMessageConverter(IList<object> domainEvents, IList<EventMessage> eventStoreEvents)
		{
			var eventMessageConverter = Substitute.For<Converter<object, EventMessage>>();
			domainEvents.Count.Repeat(i => eventMessageConverter(domainEvents[i]).Returns(eventStoreEvents[i]));
			return eventMessageConverter;
		}

		[Fact]
		public void CreateFrom_Called_ExpectFutureDomainEventsAreWrittenToEventStore()
		{
			var domainEvents = CreateAtLeastOneDomainEvent();
			var eventStoreEvents = domainEvents.Select(x => new EventMessage()).ToArray();

			var aggregateRoot = new object();
			Source.Of<object> capturedSourceEvent = (sender, args) => { };
			var prePersistenceTracker = Substitute.For<IPrePersistenceDomainEventTracker<object>>();
			prePersistenceTracker.SwitchTrackingToPersistentStore(aggregateRoot, Arg.Do<Source.Of<object>>(x => capturedSourceEvent = x));

			var eventStoreStream = Substitute.For<IEventStream>();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStream),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				StubEventMessageConverter(domainEvents, eventStoreEvents),
				DummyEventReplay()))
			{
				stream.CreateFrom(aggregateRoot);
				domainEvents.ForEach(domainEvent => capturedSourceEvent(aggregateRoot, domainEvent));
			}

			Received.InOrder(() => domainEvents.Length.Repeat(i => eventStoreStream.Add(eventStoreEvents[i])));
		}

		private static IStoreEvents StubEventStoreCreateStream(params IEventStream[] eventStoreStreams)
		{
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.CreateStream(Arg.Any<string>(), Arg.Any<string>()).Returns(eventStoreStreams.First(), eventStoreStreams.Skip(1).ToArray());
			return eventStore;
		}

		[Fact]
		public void CreateFrom_CalledWhenSwitchingThrowsException_ExpectCreatedEventStoreStreamIsStillTrackedForDisposal()
		{
			var prePersistenceTracker = Substitute.For<IPrePersistenceDomainEventTracker<object>>();
			prePersistenceTracker.When(x => x.SwitchTrackingToPersistentStore(Arg.Any<object>(), Arg.Any<Source.Of<object>>()))
				.Do(x => { throw new Exception(); });

			var eventStoreStream = Substitute.For<IEventStream>();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStream),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				stream.Invoking(x => x.CreateFrom(new object())).ShouldThrow<Exception>();
			}

			eventStoreStream.Received(1).Dispose();
		}

		[Fact]
		public void CreateFrom_CalledWithMultipleAggregateRoots_ExpectDomainEventsAreSentToCorrespondingStreams()
		{
			int numberOfAggregateRoots = IntegerGenerator.WithinExclusiveRange(2, 10);
			var aggregateRoots = numberOfAggregateRoots.Select(() => new object()).ToArray();
			var domainEvents = numberOfAggregateRoots.Select(() => new EventMessage()).ToArray();
			var eventStoreStreams = numberOfAggregateRoots.Select(() => Substitute.For<IEventStream>()).ToArray();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				StubPrePersistenceDomainEventTrackerToSourceOneDomainEventPerAggregateRoot(aggregateRoots, domainEvents),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				x => (EventMessage) x,
				DummyEventReplay()))
			{
				aggregateRoots.ForEach(aggregateRoot => stream.CreateFrom(aggregateRoot));
			}

			numberOfAggregateRoots.Repeat(i => eventStoreStreams[i].Received(1).Add(domainEvents[i]));
		}

		private static IPrePersistenceDomainEventTracker<object> StubPrePersistenceDomainEventTrackerToSourceOneDomainEventPerAggregateRoot(
			IList<object> aggregateRoots, IList<object> domainEvents)
		{
			var prePersistenceDomainEventTracker = Substitute.For<IPrePersistenceDomainEventTracker<object>>();
			aggregateRoots.Count.Repeat(i =>
				prePersistenceDomainEventTracker.SwitchTrackingToPersistentStore(
					aggregateRoots[i],
					Arg.Do<Source.Of<object>>(eventSource => eventSource(aggregateRoots[i], domainEvents[i]))));

			return prePersistenceDomainEventTracker;
		}

		[Fact]
		public void Replay_CalledWithNullAggregateRootId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var stream = new DomainEventStream<object>(
				DummyEventStore(),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				stream.Invoking(x => x.Replay(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootId");
			}
		}

		[Fact]
		public void Replay_Called_ExpectEventStoreStreamIsOpenedWithCorrectBucketIdAndAggregateRootIdPassedIn()
		{
			string bucketId = StringGenerator.AnyNonNull();
			string aggregateRootId = StringGenerator.AnyNonNull();
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.OpenStream(bucketId, aggregateRootId, int.MinValue, int.MaxValue).Returns(Substitute.For<IEventStream>());

			using (var stream = new DomainEventStream<object>(
				eventStore,
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				bucketId,
				DummyEventConverter,
				DummyEventReplay()))
			{
				stream.Replay(aggregateRootId);
			}

			eventStore.Received(1).OpenStream(bucketId, aggregateRootId, int.MinValue, int.MaxValue);
		}

		// TODO: START FROM HERE - CONTINUE ELABORATING Replay()

		[Fact]
		public void Commit_CalledWithNoExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithNonEmptyId()
		{
			var eventStoreStreams = CreateAtLeastOneEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				stream.Commit();
			}

			eventStoreStreams.ForEach(es => es.Received(1).CommitChanges(Arg.Is<Guid>(commitId => commitId != Guid.Empty)));
		}

		private static IEventStream[] CreateAtLeastOneEventStoreStream()
		{
			return CreateNumerOfEventStoreStreamsWithinExclusiveRange(1, 10);
		}

		private static IEventStream[] CreateNumerOfEventStoreStreamsWithinExclusiveRange(int min, int halfOpenMax)
		{
			int numberOfEventStoreStreams = IntegerGenerator.WithinExclusiveRange(min, halfOpenMax);
			return numberOfEventStoreStreams.Select(() => Substitute.For<IEventStream>()).ToArray();
		}

		[Fact]
		public void Commit_CalledWithNoExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithSameId()
		{
			var commitIds = new List<Guid>();
			var eventStoreStreams = CreateAtLeastTwoEventStoreStreams();
			eventStoreStreams.ForEach(es => es.CommitChanges(Arg.Do<Guid>(commitIds.Add)));
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				stream.Commit();
			}

			commitIds.Distinct().Count().Should().Be(1);
		}

		private static IEventStream[] CreateAtLeastTwoEventStoreStreams()
		{
			return CreateNumerOfEventStoreStreamsWithinExclusiveRange(2, 10);
		}

		[Fact]
		public void Commit_CalledMultipleTimesWithNoExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithDifferentIdsPerCall()
		{
			var commitIds = new List<Guid>();
			var eventStoreStreams = CreateAtLeastTwoEventStoreStreams();
			eventStoreStreams.ForEach(es => es.CommitChanges(Arg.Do<Guid>(commitIds.Add)));
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				int numberOfCommits = IntegerGenerator.WithinExclusiveRange(2, 10);
				numberOfCommits.Repeat(stream.Commit);
				commitIds.Distinct().Count().Should().Be(numberOfCommits);
			}
		}

		[Fact]
		public void Commit_CalledWithExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithSameId()
		{
			var eventStoreStreams = CreateAtLeastOneEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				var commitId = Guid.NewGuid();
				stream.Commit(commitId);
				eventStoreStreams.ForEach(es => es.Received(1).CommitChanges(commitId));
			}
		}

		[Fact]
		public void ClearUncommitted_Called_ExpectAllEventStoreStreamsAreCleared()
		{
			var eventStoreStreams = CreateAtLeastOneEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				stream.ClearUncommitted();
				eventStoreStreams.ForEach(es => es.Received(1).ClearChanges());
			}
		}

		[Fact]
		public void Dispose_Called_ExpectCreatedEventStoreStreamsAreDisposed()
		{
			var eventStoreStreams = CreateAtLeastOneEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
			}

			eventStoreStreams.ForEach(es => es.Received(1).Dispose());
		}

		[Fact]
		public void Dispose_CalledMultipleTimes_ExpectCreatedEventStoreStreamsAreNotDisposedMoreThanOnce()
		{
			var eventStoreStreams = CreateAtLeastOneEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				DummyEventReplay()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				stream.Dispose();
			}

			eventStoreStreams.ForEach(es => es.Received(1).Dispose());
		}

		// TODO: Replay() - needs writing
		// TODO: Dispose() - OpenStream() calls also need disposing
		// TODO: Option to allow auto-commit on Dispose ?
		// TODO: Option to allow auto-commit after every Domain Event ?
	}
}
