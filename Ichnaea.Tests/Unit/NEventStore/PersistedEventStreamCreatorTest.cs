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
	public class PersistedEventStreamCreatorTest
	{
		[Fact]
		public void Constructor_CalledWithNullEventStore_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				null,
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventStore");
		}

		private static IPrePersistenceDomainEventTracker<T> DummyPrePersistenceTracker<T>() where T: class
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

		[Fact]
		public void Constructor_CalledWithNullPrePersistenceDomainEventTracker_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				DummyEventStore(),
				null,
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());

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
				DummyEventStore(),
				DummyPrePersistenceTracker<object>(),
				null,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootIdGetter");
		}

		[Fact]
		public void Constructor_CalledWithNullBucketId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				DummyEventStore(),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				null,
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("bucketId");
		}

		[Fact]
		public void Constructor_CalledWithNullDomainEventToPersistedEventConverter_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				DummyEventStore(),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				null,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("domainEventToPersistedEventConverter");
		}

		[Fact]
		public void Constructor_CalledWithNullPersistedEventReplay_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(
				DummyEventStore(), DummyPrePersistenceTracker<object>(), DummyAggregateRootIdGetter, DummyBucketId(), DummyEventConverter, null);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("persistedEventReplay");
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
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.Invoking(x => x.CreateFrom(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
			}
		}

		[Fact]
		public void CreateFrom_Called_ExpectEventStoreStreamIsCreatedWithCorrectBucketIdAndIdFromAggregateRoot()
		{
			string bucketId = DummyBucketId();
			string aggregateRootId = DummyAggregateRootId();
			var eventStore = MockEventStore();
			eventStore.CreateStream(bucketId, aggregateRootId).Returns(Substitute.For<IEventStream>());

			var aggregateRoot = new object();
			using (var stream = new DomainEventStream<object>(
				eventStore,
				DummyPrePersistenceTracker<object>(),
				StubAggregateRootIdGetter(aggregateRoot, aggregateRootId),
				bucketId,
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.CreateFrom(aggregateRoot);
				eventStore.Received(1).CreateStream(bucketId, aggregateRootId);
			}
		}

		private static string DummyAggregateRootId()
		{
			return StringGenerator.AnyNonNull();
		}

		private static IStoreEvents MockEventStore()
		{
			return Substitute.For<IStoreEvents>();
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
			var prePersistenceTracker = MockPrePersistenceTracker();
			using (var stream = new DomainEventStream<object>(
				DummyEventStore(),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				var aggregateRoot = new object();
				stream.CreateFrom(aggregateRoot);
				prePersistenceTracker.Received(1).SwitchTrackingToPersistentStore(aggregateRoot, Arg.Is<Source.Of<object>>(x => x != null));
			}
		}

		private static IPrePersistenceDomainEventTracker<object> MockPrePersistenceTracker()
		{
			return Substitute.For<IPrePersistenceDomainEventTracker<object>>();
		}

		[Fact]
		public void CreateFrom_Called_ExpectDomainEventsFromPrePersistenceTrackerAreWrittenToTheEventStore()
		{
			var domainEvents = CreateAtLeastOneDomainEvent();
			var persistedEvents = domainEvents.Select(x => new EventMessage()).ToArray();

			string bucketId = DummyBucketId();
			string aggregateRootId = DummyAggregateRootId();
			var eventStoreStream = Substitute.For<IEventStream>();
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.CreateStream(bucketId, aggregateRootId).Returns(eventStoreStream);

			var aggregateRoot = new object();
			using (var stream = new DomainEventStream<object>(
				eventStore,
				StubPrePersistenceDomainEventTracker(aggregateRoot, domainEvents),
				StubAggregateRootIdGetter(aggregateRoot, aggregateRootId),
				bucketId,
				StubEventConverter(domainEvents, persistedEvents),
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.CreateFrom(aggregateRoot);
				Received.InOrder(() => domainEvents.Length.Repeat(i => eventStoreStream.Add(persistedEvents[i])));
			}
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

		private static Converter<object, EventMessage> StubEventConverter(IList<object> domainEvents, IList<EventMessage> persistedEvents)
		{
			var eventConverter = Substitute.For<Converter<object, EventMessage>>();
			domainEvents.Count.Repeat(i => eventConverter(domainEvents[i]).Returns(persistedEvents[i]));
			return eventConverter;
		}

		[Fact]
		public void CreateFrom_Called_ExpectFutureDomainEventsAreWrittenToEventStore()
		{
			var domainEvents = CreateAtLeastOneDomainEvent();
			var persistedEvents = domainEvents.Select(x => new EventMessage()).ToArray();

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
				StubEventConverter(domainEvents, persistedEvents),
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.CreateFrom(aggregateRoot);
				domainEvents.ForEach(domainEvent => capturedSourceEvent(aggregateRoot, domainEvent));
				Received.InOrder(() => domainEvents.Length.Repeat(i => eventStoreStream.Add(persistedEvents[i])));
			}
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
			var prePersistenceTracker = MockPrePersistenceTracker();
			prePersistenceTracker.When(x => x.SwitchTrackingToPersistentStore(Arg.Any<object>(), Arg.Any<Source.Of<object>>()))
				.Do(x => { throw new Exception(); });

			var eventStoreStream = MockEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStream),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.Invoking(x => x.CreateFrom(new object())).ShouldThrow<Exception>();
			}

			eventStoreStream.Received(1).Dispose();
		}

		private static IEventStream MockEventStoreStream()
		{
			return Substitute.For<IEventStream>();
		}

		[Fact]
		public void CreateFrom_CalledWithMultipleAggregateRoots_ExpectDomainEventsAreSentToCorrespondingStreams()
		{
			int numberOfAggregateRoots = IntegerGenerator.WithinExclusiveRange(2, 10);
			var aggregateRoots = numberOfAggregateRoots.Select(() => new object()).ToArray();
			var domainEvents = numberOfAggregateRoots.Select(() => new EventMessage()).ToArray();
			var eventStoreStreams = numberOfAggregateRoots.Select(MockEventStoreStream).ToArray();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				StubPrePersistenceDomainEventTrackerToSourceOneDomainEventPerAggregateRoot(aggregateRoots, domainEvents),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				x => (EventMessage) x,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				aggregateRoots.ForEach(aggregateRoot => stream.CreateFrom(aggregateRoot));
				numberOfAggregateRoots.Repeat(i => eventStoreStreams[i].Received(1).Add(domainEvents[i]));
			}
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
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.Invoking(x => x.Replay(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootId");
			}
		}

		[Fact]
		public void Replay_Called_ExpectEventStoreStreamIsOpenedWithCorrectBucketIdAndAggregateRootIdPassedIn()
		{
			string bucketId = DummyBucketId();
			string aggregateRootId = DummyAggregateRootId();
			var eventStorStreamWithCommittedEvents = StubEventStoreStreamForAtLeastOneCommittedEvent();
			var eventStore = MockEventStore();
			eventStore.OpenStream(bucketId, aggregateRootId, int.MinValue, int.MaxValue).Returns(eventStorStreamWithCommittedEvents);

			using (var stream = new DomainEventStream<object>(
				eventStore,
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				bucketId,
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.Replay(aggregateRootId);
				eventStore.Received(1).OpenStream(bucketId, aggregateRootId, int.MinValue, int.MaxValue);
			}
		}

		private static IEventStream StubEventStoreStreamForAtLeastOneCommittedEvent()
		{
			int numberOfCommittedEvents = IntegerGenerator.WithinExclusiveRange(1, 10);
			return StubEventStoreStreamForCommittedEvents(numberOfCommittedEvents.Select(() => new EventMessage()).ToList());
		}

		private static IEventStream StubEventStoreStreamForCommittedEvents(ICollection<EventMessage> eventStoreCommittedEvents)
		{
			var eventStoreStream = Substitute.For<IEventStream>();
			eventStoreStream.CommittedEvents.Returns(eventStoreCommittedEvents);
			return eventStoreStream;
		}

		[Fact]
		public void Replay_Called_ExpectCommittedEventsFromOpenedEventStoreStreamAreAggregatedAndReturned()
		{
			int numberOfCommittedEvents = IntegerGenerator.WithinExclusiveRange(1, 10);
			var committedEvents = numberOfCommittedEvents.Select(() => new EventMessage()).ToList();
			var rootUnderConstruction = numberOfCommittedEvents.Select(() => new object()).ToArray();
			var aggregateRoot = rootUnderConstruction.Last();

			string bucketId = DummyBucketId();
			string aggregateRootId = DummyAggregateRootId();

			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStreamForCommittedEvents(bucketId, aggregateRootId, committedEvents),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				bucketId,
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.StubForCommittedEventSequence(committedEvents, rootUnderConstruction)))
			{
				stream.Replay(aggregateRootId).Should().BeSameAs(aggregateRoot);
			}
		}

		private static IStoreEvents StubEventStoreOpenStreamForCommittedEvents(
			string bucketId, string aggregateRootId, ICollection<EventMessage> eventStoreCommittedEvents)
		{
			return StubEventStoreOpenStream(bucketId, aggregateRootId, StubEventStoreStreamForCommittedEvents(eventStoreCommittedEvents));
		}

		private static IStoreEvents StubEventStoreOpenStream(string bucketId, string aggregateRootId, IEventStream eventStoreStream)
		{
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.OpenStream(bucketId, aggregateRootId, int.MinValue, int.MaxValue).Returns(eventStoreStream);
			return eventStore;
		}

		[Fact]
		public void Replay_CalledWhenFinalReplayReturnsNull_ExpectDomainEventStreamCannotBeReplayedException()
		{
			var replayReturningNullAggregateRoot = StubEventReplayToReturnNullAggregateRoot();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStreamForAtLeastOneCommittedEvent(),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				replayReturningNullAggregateRoot))
			{
				stream.Invoking(x => x.Replay(DummyAggregateRootId())).ShouldThrow<DomainEventStreamCannotBeReplayedException>();
			}
		}

		private static PersistedEventToDomainEventReplayAdapter<object> StubEventReplayToReturnNullAggregateRoot()
		{
			var replayReturningNullAggregateRoot = PersistedEventToDomainEventReplayAdapterTestDoubles.Stub();
			replayReturningNullAggregateRoot.CanReplay(Arg.Any<object>(), Arg.Any<EventMessage>()).Returns(true);
			replayReturningNullAggregateRoot.Replay(Arg.Any<object>(), Arg.Any<EventMessage>()).Returns(null);
			return replayReturningNullAggregateRoot;
		}

		private static IStoreEvents StubEventStoreOpenStreamForAtLeastOneCommittedEvent()
		{
			return StubEventStoreOpenStream(StubEventStoreStreamForAtLeastOneCommittedEvent());
		}

		private static IStoreEvents StubEventStoreOpenStream(IEventStream eventStoreStream)
		{
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.OpenStream(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>()).Returns(eventStoreStream);
			return eventStore;
		}

		[Fact]
		public void Replay_CalledWhenAnEventCannotBeReplayed_ExpectDomainEventStreamCannotBeReplayedException()
		{
			var eventStoreCommittedEvents = CreateAtLeastOneDummyPersistedEvent();
			var replayUnableToHandleAllEvents = StubEventReplayForOneUnhandledEvent(eventStoreCommittedEvents);
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStreamForCommittedEvents(eventStoreCommittedEvents),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				replayUnableToHandleAllEvents))
			{
				stream.Invoking(x => x.Replay(DummyAggregateRootId())).ShouldThrow<DomainEventStreamCannotBeReplayedException>();
			}
		}

		private static PersistedEventToDomainEventReplayAdapter<object> StubEventReplayForOneUnhandledEvent(
			IEnumerable<EventMessage> eventStoreCommittedEvents)
		{
			var invalidEvent = eventStoreCommittedEvents.AnyItem();
			var replayUnableToHandleAllEvents = PersistedEventToDomainEventReplayAdapterTestDoubles.Stub();
			replayUnableToHandleAllEvents.CanReplay(Arg.Any<object>(), Arg.Is(invalidEvent)).Returns(false);
			replayUnableToHandleAllEvents.CanReplay(Arg.Any<object>(), Arg.Is<EventMessage>(x => x != invalidEvent)).Returns(true);
			replayUnableToHandleAllEvents.Replay(Arg.Any<object>(), Arg.Any<EventMessage>()).Returns(_ => new object());
			return replayUnableToHandleAllEvents;
		}

		private static EventMessage[] CreateAtLeastOneDummyPersistedEvent()
		{
			int numberOfEventMessages = IntegerGenerator.WithinExclusiveRange(1, 10);
			return numberOfEventMessages.Select(DummyPersistedEvent).ToArray();
		}

		private static EventMessage DummyPersistedEvent()
		{
			return new EventMessage();
		}

		private static IStoreEvents StubEventStoreOpenStreamForCommittedEvents(ICollection<EventMessage> eventStoreCommittedEvents)
		{
			return StubEventStoreOpenStream(StubEventStoreStreamForCommittedEvents(eventStoreCommittedEvents));
		}

		[Fact]
		public void Replay_CalledWhenNoEventsHaveBeenPersistedToEventStore_ExpectDomainEventStreamCannotBeReplayedException()
		{
			var eventStoreCommittedEvents = new EventMessage[0];
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStreamForCommittedEvents(eventStoreCommittedEvents),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.Invoking(x => x.Replay(DummyAggregateRootId())).ShouldThrow<DomainEventStreamCannotBeReplayedException>();
			}
		}

		[Fact]
		public void Replay_Called_ExpectPrePersistenceTrackerIsSwitchedToTheEventStoreForTheAggregateRoot()
		{
			var prePersistenceTracker = MockPrePersistenceTracker();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStreamForAtLeastOneCommittedEvent(),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				var aggregateRoot = stream.Replay(DummyAggregateRootId());
				prePersistenceTracker.Received(1).SwitchTrackingToPersistentStore(aggregateRoot, Arg.Is<Source.Of<object>>(x => x != null));
			}
		}

		[Fact]
		public void Replay_CalledWhenFinalReplayReturnsNull_ExpectPrePersistenceTrackerIsNotSwitchedToTheEventStore()
		{
			var prePersistenceTracker = MockPrePersistenceTracker();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStreamForAtLeastOneCommittedEvent(),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				StubEventReplayToReturnNullAggregateRoot()))
			{
				stream.Invoking(x => x.Replay(DummyAggregateRootId())).ShouldThrow<DomainEventStreamCannotBeReplayedException>();
				prePersistenceTracker.DidNotReceive().SwitchTrackingToPersistentStore(Arg.Any<object>(), Arg.Any<Source.Of<object>>());
			}
		}

		[Fact]
		public void Replay_Called_ExpectFutureDomainEventsAreWrittenToEventStore()
		{
			var domainEvents = CreateAtLeastOneDomainEvent();
			var persistedEvents = domainEvents.Select(x => new EventMessage()).ToArray();

			Source.Of<object> capturedSourceEvent = (sender, args) => { };
			var prePersistenceTracker = Substitute.For<IPrePersistenceDomainEventTracker<object>>();
			prePersistenceTracker.SwitchTrackingToPersistentStore(Arg.Any<object>(), Arg.Do<Source.Of<object>>(x => capturedSourceEvent = x));

			var eventStoreStream = StubEventStoreStreamForAtLeastOneCommittedEvent();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStream(eventStoreStream),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				StubEventConverter(domainEvents, persistedEvents),
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				var aggregateRoot = stream.Replay(DummyAggregateRootId());
				domainEvents.ForEach(domainEvent => capturedSourceEvent(aggregateRoot, domainEvent));
				Received.InOrder(() => domainEvents.Length.Repeat(i => eventStoreStream.Add(persistedEvents[i])));
			}
		}

		[Fact]
		public void Replay_CalledWhenNoEventsHaveBeenPersistedToEventStore_ExpectCreatedEventStoreStreamIsStillTrackedForDisposal()
		{
			var eventStoreCommittedEvents = new EventMessage[0];
			var eventStoreStream = MockEventStoreStreamStubbedForCommittedEvents(eventStoreCommittedEvents);
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStream(eventStoreStream),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.Invoking(x => x.Replay(DummyAggregateRootId())).ShouldThrow<DomainEventStreamCannotBeReplayedException>();
			}

			eventStoreStream.Received(1).Dispose();
		}

		private static IEventStream MockEventStoreStreamStubbedForCommittedEvents(ICollection<EventMessage> committedEvents)
		{
			return StubEventStoreStreamForCommittedEvents(committedEvents);
		}

		[Fact]
		public void Replay_CalledWhenAnEventCannotBeReplayed_ExpectCreatedEventStoreStreamIsStillTrackedForDisposal()
		{
			var eventStoreCommittedEvents = CreateAtLeastOneDummyPersistedEvent();
			var replayUnableToHandleAllEvents = StubEventReplayForOneUnhandledEvent(eventStoreCommittedEvents);
			var eventStoreStream = MockEventStoreStreamStubbedForCommittedEvents(eventStoreCommittedEvents);
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStream(eventStoreStream),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				replayUnableToHandleAllEvents))
			{
				stream.Invoking(x => x.Replay(DummyAggregateRootId())).ShouldThrow<DomainEventStreamCannotBeReplayedException>();
			}

			eventStoreStream.Received(1).Dispose();
		}

		[Fact]
		public void Replay_CalledWhenFinalReplayReturnsNull_ExpectCreatedEventStoreStreamIsStillTrackedForDisposal()
		{
			var replayReturningNullAggregateRoot = StubEventReplayToReturnNullAggregateRoot();
			var eventStoreStream = MockEventStoreStreamStubbedForAtLeastOneCommittedEvent();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStream(eventStoreStream),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				replayReturningNullAggregateRoot))
			{
				stream.Invoking(x => x.Replay(DummyAggregateRootId())).ShouldThrow<DomainEventStreamCannotBeReplayedException>();
			}

			eventStoreStream.Received(1).Dispose();
		}

		private static IEventStream MockEventStoreStreamStubbedForAtLeastOneCommittedEvent()
		{
			return StubEventStoreStreamForAtLeastOneCommittedEvent();
		}

		[Fact]
		public void Replay_CalledWhenSwitchingThrowsException_ExpectCreatedEventStoreStreamIsStillTrackedForDisposal()
		{
			var prePersistenceTracker = Substitute.For<IPrePersistenceDomainEventTracker<object>>();
			prePersistenceTracker.When(x => x.SwitchTrackingToPersistentStore(Arg.Any<object>(), Arg.Any<Source.Of<object>>()))
				.Do(x => { throw new Exception(); });

			var eventStoreStream = MockEventStoreStreamStubbedForAtLeastOneCommittedEvent();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStream(eventStoreStream),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.Invoking(x => x.Replay(DummyAggregateRootId())).ShouldThrow<Exception>();
			}

			eventStoreStream.Received(1).Dispose();
		}

		[Fact]
		public void Commit_CalledWithNoExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithNonEmptyId()
		{
			var eventStoreStreams = MockAtLeastOneEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				stream.Commit();
				eventStoreStreams.ForEach(es => es.Received(1).CommitChanges(Arg.Is<Guid>(commitId => commitId != Guid.Empty)));
			}
		}

		private static IEventStream[] MockAtLeastOneEventStoreStream()
		{
			return CreateNumerOfEventStoreStreamsWithinExclusiveRange(1, 10);
		}

		private static IEventStream[] CreateNumerOfEventStoreStreamsWithinExclusiveRange(int min, int halfOpenMax)
		{
			int numberOfEventStoreStreams = IntegerGenerator.WithinExclusiveRange(min, halfOpenMax);
			return numberOfEventStoreStreams.Select(MockEventStoreStream).ToArray();
		}

		[Fact]
		public void Commit_CalledWithNoExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithSameId()
		{
			var commitIds = new List<Guid>();
			var eventStoreStreams = StubAtLeastTwoEventStoreStreams();
			eventStoreStreams.ForEach(es => es.CommitChanges(Arg.Do<Guid>(commitIds.Add)));
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				stream.Commit();
				commitIds.Distinct().Count().Should().Be(1);
			}
		}

		private static IEventStream[] StubAtLeastTwoEventStoreStreams()
		{
			return CreateNumerOfEventStoreStreamsWithinExclusiveRange(2, 10);
		}

		// TODO: NEED TO TEST FOR Commit AND ClearUncommitted WITH OpenStream...

		[Fact]
		public void Commit_CalledMultipleTimesWithNoExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithDifferentIdsPerCall()
		{
			var commitIds = new List<Guid>();
			var eventStoreStreams = StubAtLeastTwoEventStoreStreams();
			eventStoreStreams.ForEach(es => es.CommitChanges(Arg.Do<Guid>(commitIds.Add)));
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
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
			var eventStoreStreams = MockAtLeastOneEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
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
			var eventStoreStreams = MockAtLeastOneEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				stream.ClearUncommitted();
				eventStoreStreams.ForEach(es => es.Received(1).ClearChanges());
			}
		}

		[Fact]
		public void Dispose_Called_ExpectCreatedEventStoreStreamsAreDisposed()
		{
			var eventStoreStreams = MockAtLeastOneEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
			}

			eventStoreStreams.ForEach(es => es.Received(1).Dispose());
		}

		[Fact]
		public void Dispose_CalledMultipleTimes_ExpectCreatedEventStoreStreamsAreNotDisposedMoreThanOnce()
		{
			var eventStoreStreams = MockAtLeastOneEventStoreStream();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				stream.Dispose();
			}

			eventStoreStreams.ForEach(es => es.Received(1).Dispose());
		}

		[Fact]
		public void Dispose_Called_ExpectOpenedEventStoreStreamsAreDisposed()
		{
			var eventStoreStreams = MockAtLeastOneEventStoreStreamEachStubbedForAtLeastOneCommittedEvent();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				eventStoreStreams.ForEach(x => stream.Replay(DummyAggregateRootId()));
			}

			eventStoreStreams.ForEach(es => es.Received(1).Dispose());
		}

		private static IEventStream[] MockAtLeastOneEventStoreStreamEachStubbedForAtLeastOneCommittedEvent()
		{
			return MockNumerOfEventStoreStreamsWithinExclusiveRangeEachStubbedForAtLeastOneCommittedEvent(1, 10);
		}

		private static IEventStream[] MockNumerOfEventStoreStreamsWithinExclusiveRangeEachStubbedForAtLeastOneCommittedEvent(int min, int halfOpenMax)
		{
			int numberOfEventStoreStreams = IntegerGenerator.WithinExclusiveRange(min, halfOpenMax);
			return numberOfEventStoreStreams.Select(StubEventStoreStreamForAtLeastOneCommittedEvent).ToArray();
		}

		private static IStoreEvents StubEventStoreOpenStream(params IEventStream[] eventStoreStreams)
		{
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.OpenStream(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
				.Returns(eventStoreStreams.First(), eventStoreStreams.Skip(1).ToArray());

			return eventStore;
		}

		[Fact]
		public void Dispose_CalledMultipleTimes_ExpectOpenedEventStoreStreamsAreNotDisposedMoreThanOnce()
		{
			var eventStoreStreams = MockAtLeastOneEventStoreStreamEachStubbedForAtLeastOneCommittedEvent();
			using (var stream = new DomainEventStream<object>(
				StubEventStoreOpenStream(eventStoreStreams),
				DummyPrePersistenceTracker<object>(),
				DummyAggregateRootIdGetter,
				DummyBucketId(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				eventStoreStreams.ForEach(x => stream.Replay(DummyAggregateRootId()));
				stream.Dispose();
			}

			eventStoreStreams.ForEach(es => es.Received(1).Dispose());
		}

		[Fact]
		public void ExpectGarbageCollectableAggregateRootsAreNotArtificiallyKeptAliveByTracking()
		{
			///////// TODO
			using (var tracker = new InMemoryDomainEventTracker<object>())
			{
				object aggregateRoot = new object();
				tracker.AggregateRootCreated(aggregateRoot, new object());
				var weakAggregateRoot = new WeakReference<object>(aggregateRoot);
				aggregateRoot = null;
				Collect.Garbage();
				weakAggregateRoot.TryGetTarget(out aggregateRoot).Should().BeFalse();
			}
		}

		[Fact]
		public void ExpectGarbageCollectableStreamsAreNotArtificiallyKeptAliveByTracking()
		{
			///////// TODO
		}

		// TODO: Option to allow auto-commit on Dispose ?
		// TODO: Option to allow auto-commit after every Domain Event ?
	}
}
