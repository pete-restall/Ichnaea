using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using NEventStore;
using NSubstitute;
using Restall.Ichnaea.NEventStore;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit.NEventStore
{
	public class PersistedEventStreamOpenerTest
	{
		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullEventStore_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventStreamOpener<object, string>(
				null,
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventStore");
		}

		private static string DummyBucketId()
		{
			return StringGenerator.AnyNonNull();
		}

		private static string DummyAggregateRootIdConverter<T>(T aggregateRootId)
		{
			return aggregateRootId.ToString();
		}

		private static EventMessage DummyEventConverter(object domainEvent)
		{
			return new EventMessage();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullBucketId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventStreamOpener<object, string>(
				DummyEventStore(),
				null,
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("bucketId");
		}

		private static IStoreEvents DummyEventStore()
		{
			return Substitute.For<IStoreEvents>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullAggregateRootIdToStringConverter_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventStreamOpener<object, string>(
				DummyEventStore(),
				DummyBucketId(),
				null,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootIdToStringConverter");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullPostPersistenceDomainEventTracker_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventStreamOpener<object, string>(
				DummyEventStore(),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				null,
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("postPersistenceDomainEventTracker");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullDomainEventToPersistedEventConverter_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventStreamOpener<object, string>(
				DummyEventStore(),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				null,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("domainEventToPersistedEventConverter");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullPersistedEventReplay_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventStreamOpener<object, string>(
				DummyEventStore(),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				null);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("persistedEventReplay");
		}

		[Fact]
		public void Replay_CalledWithNullAggregateRootId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var stream = new PersistedEventStreamOpener<object, string>(
				DummyEventStore(),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
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
			var aggregateRootId = Guid.NewGuid();
			string aggregateRootIdAsString = DummyAggregateRootId();
			var eventStoreStreamWithCommittedEvents = StubEventStoreStreamForAtLeastOneCommittedEvent();
			var eventStore = MockEventStore();
			eventStore.OpenStream(bucketId, aggregateRootIdAsString, int.MinValue, int.MaxValue).Returns(eventStoreStreamWithCommittedEvents);

			using (var stream = new PersistedEventStreamOpener<object, Guid>(
				eventStore,
				bucketId,
				StubAggregateRootIdToStringConverter(aggregateRootId, aggregateRootIdAsString),
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.Replay(aggregateRootId);
				eventStore.Received(1).OpenStream(bucketId, aggregateRootIdAsString, int.MinValue, int.MaxValue);
			}
		}

		private static string DummyAggregateRootId()
		{
			return StringGenerator.AnyNonNull();
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

		private static IStoreEvents MockEventStore()
		{
			return Substitute.For<IStoreEvents>();
		}

		private static Converter<T, string> StubAggregateRootIdToStringConverter<T>(T aggregateRootId, string aggregateRootIdAsString)
		{
			return x => x.Equals(aggregateRootId) ? aggregateRootIdAsString : StringGenerator.AnyNonNull();
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

			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStreamForCommittedEvents(bucketId, aggregateRootId, committedEvents),
				bucketId,
				StubAggregateRootIdToStringConverter(aggregateRootId),
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
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

		private static Converter<T, string> StubAggregateRootIdToStringConverter<T>(T aggregateRootId)
		{
			return x => aggregateRootId.ToString();
		}

		[Fact]
		public void Replay_CalledWhenFinalReplayReturnsNull_ExpectDomainEventStreamCannotBeReplayedExceptionWithSameAggregateRootId()
		{
			var replayReturningNullAggregateRoot = StubEventReplayToReturnNullAggregateRoot();
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStreamForAtLeastOneCommittedEvent(),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				replayReturningNullAggregateRoot))
			{
				var aggregateRootId = DummyAggregateRootId();
				stream.Invoking(x => x.Replay(aggregateRootId))
					.ShouldThrow<DomainEventStreamCannotBeReplayedException>().And.AggregateRootId.Should().BeSameAs(aggregateRootId);
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
		public void Replay_CalledWhenAnEventCannotBeReplayed_ExpectDomainEventStreamCannotBeReplayedExceptionWithSameAggregateRootId()
		{
			var eventStoreCommittedEvents = CreateAtLeastOneDummyPersistedEvent();
			var replayUnableToHandleAllEvents = StubEventReplayForOneUnhandledEvent(eventStoreCommittedEvents);
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStreamForCommittedEvents(eventStoreCommittedEvents),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				replayUnableToHandleAllEvents))
			{
				var aggregateRootId = DummyAggregateRootId();
				stream.Invoking(x => x.Replay(aggregateRootId))
					.ShouldThrow<DomainEventStreamCannotBeReplayedException>().And.AggregateRootId.Should().BeSameAs(aggregateRootId);
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
		public void Replay_CalledWhenNoEventsHaveBeenPersistedToEventStore_ExpectDomainEventStreamCannotBeReplayedExceptionWithSameAggregateRootId()
		{
			var eventStoreCommittedEvents = new EventMessage[0];
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStreamForCommittedEvents(eventStoreCommittedEvents),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				var aggregateRootId = DummyAggregateRootId();
				stream.Invoking(x => x.Replay(aggregateRootId))
					.ShouldThrow<DomainEventStreamCannotBeReplayedException>().And.AggregateRootId.Should().BeSameAs(aggregateRootId);
			}
		}

		[Fact]
		public void Replay_Called_ExpectPostPersistenceTrackingIsForTheAggregateRoot()
		{
			var postPersistenceTracker = MockPostPersistenceTracker();
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStreamForAtLeastOneCommittedEvent(),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				postPersistenceTracker,
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				var aggregateRoot = stream.Replay(DummyAggregateRootId());
				postPersistenceTracker.Received(1).TrackToPersistentStore(aggregateRoot, Arg.Is<Source.Of<object>>(x => x != null));
			}
		}

		private static IPostPersistenceDomainEventTracker<object> MockPostPersistenceTracker()
		{
			return Substitute.For<IPostPersistenceDomainEventTracker<object>>();
		}

		[Fact]
		public void Replay_CalledWhenFinalReplayReturnsNull_ExpectNoPostPersistenceTracking()
		{
			var postPersistenceTracker = MockPostPersistenceTracker();
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStreamForAtLeastOneCommittedEvent(),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				postPersistenceTracker,
				DummyEventConverter,
				StubEventReplayToReturnNullAggregateRoot()))
			{
				stream.Invoking(x => x.Replay(DummyAggregateRootId())).ShouldThrow<DomainEventStreamCannotBeReplayedException>();
				postPersistenceTracker.DidNotReceive().TrackToPersistentStore(Arg.Any<object>(), Arg.Any<Source.Of<object>>());
			}
		}

		[Fact]
		public void Replay_Called_ExpectFutureDomainEventsAreWrittenToEventStore()
		{
			var domainEvents = CreateAtLeastOneDomainEvent();
			var persistedEvents = domainEvents.Select(x => new EventMessage()).ToArray();

			Source.Of<object> capturedSourceEvent = (sender, args) => { };
			var postPersistenceTracker = Substitute.For<IPostPersistenceDomainEventTracker<object>>();
			postPersistenceTracker.TrackToPersistentStore(Arg.Any<object>(), Arg.Do<Source.Of<object>>(x => capturedSourceEvent = x));

			var eventStoreStream = StubEventStoreStreamForAtLeastOneCommittedEvent();
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStream(eventStoreStream),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				postPersistenceTracker,
				StubEventConverter(domainEvents, persistedEvents),
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				var aggregateRoot = stream.Replay(DummyAggregateRootId());
				domainEvents.ForEach(domainEvent => capturedSourceEvent(aggregateRoot, domainEvent));
				Received.InOrder(() => domainEvents.Length.Repeat(i => eventStoreStream.Add(persistedEvents[i])));
			}
		}

		private static object[] CreateAtLeastOneDomainEvent()
		{
			int numberOfDomainEvents = IntegerGenerator.WithinExclusiveRange(1, 10);
			return numberOfDomainEvents.Select(() => new object()).ToArray();
		}

		private static Converter<object, EventMessage> StubEventConverter(IList<object> domainEvents, IList<EventMessage> persistedEvents)
		{
			var eventConverter = Substitute.For<Converter<object, EventMessage>>();
			domainEvents.Count.Repeat(i => eventConverter(domainEvents[i]).Returns(persistedEvents[i]));
			return eventConverter;
		}

		[Fact]
		public void Replay_CalledWhenNoEventsHaveBeenPersistedToEventStore_ExpectCreatedEventStoreStreamIsStillTrackedForDisposal()
		{
			var eventStoreCommittedEvents = new EventMessage[0];
			var eventStoreStream = MockEventStoreStreamStubbedForCommittedEvents(eventStoreCommittedEvents);
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStream(eventStoreStream),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
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
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStream(eventStoreStream),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
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
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStream(eventStoreStream),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
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
			var postPersistenceTracker = Substitute.For<IPostPersistenceDomainEventTracker<object>>();
			postPersistenceTracker.When(x => x.TrackToPersistentStore(Arg.Any<object>(), Arg.Any<Source.Of<object>>()))
				.Do(x => { throw new Exception(); });

			var eventStoreStream = MockEventStoreStreamStubbedForAtLeastOneCommittedEvent();
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStream(eventStoreStream),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				postPersistenceTracker,
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				stream.Invoking(x => x.Replay(DummyAggregateRootId())).ShouldThrow<Exception>();
			}

			eventStoreStream.Received(1).Dispose();
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = CodeAnalysisJustification.EnumerableIsMaterialisedBeforeDisposal)]
		public void Dispose_Called_ExpectOpenedEventStoreStreamsAreDisposed()
		{
			var eventStoreStreams = MockAtLeastOneEventStoreStreamEachStubbedForAtLeastOneCommittedEvent();
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStream(eventStoreStreams),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				eventStoreStreams.ForEach(x => stream.Replay(DummyAggregateRootId()));
			}

			eventStoreStreams.ForEach(stream => stream.Received(1).Dispose());
		}

		private static IEventStream[] MockAtLeastOneEventStoreStreamEachStubbedForAtLeastOneCommittedEvent()
		{
			return CreateNumerOfMockEventStoreStreamsWithinExclusiveRangeEachStubbedForAtLeastOneCommittedEvent(1, 10);
		}

		private static IEventStream[] CreateNumerOfMockEventStoreStreamsWithinExclusiveRangeEachStubbedForAtLeastOneCommittedEvent(int min, int halfOpenMax)
		{
			int numberOfEventStoreStreams = IntegerGenerator.WithinExclusiveRange(min, halfOpenMax);
			return numberOfEventStoreStreams.Select(MockEventStoreStreamStubbedForAtLeastOneCommittedEvent).ToArray();
		}

		private static IStoreEvents StubEventStoreOpenStream(params IEventStream[] eventStoreStreams)
		{
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.OpenStream(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
				.Returns(eventStoreStreams.First(), eventStoreStreams.Skip(1).ToArray());

			return eventStore;
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = CodeAnalysisJustification.EnumerableIsMaterialisedBeforeDisposal)]
		public void Dispose_CalledMultipleTimes_ExpectCreatedEventStoreStreamsAreNotDisposedMoreThanOnce()
		{
			var eventStoreStreams = MockAtLeastOneEventStoreStreamEachStubbedForAtLeastOneCommittedEvent();
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStream(eventStoreStreams),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				eventStoreStreams.ForEach(x => stream.Replay(DummyAggregateRootId()));
				stream.Dispose();
			}

			eventStoreStreams.ForEach(stream => stream.Received(1).Dispose());
		}

		[Fact]
		public void ExpectGarbageCollectableAggregateRootsAreNotArtificiallyKeptAliveByTracking()
		{
			using (var stream = new PersistedEventStreamOpener<object, string>(
				StubEventStoreOpenStreamForAtLeastOneCommittedEvent(),
				DummyBucketId(),
				DummyAggregateRootIdConverter,
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyEventConverter,
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy()))
			{
				var weakAggregateRoot = WeakAggregateRootFromReplay(stream);
				Collect.Garbage();

				object aggregateRoot;
				weakAggregateRoot.TryGetTarget(out aggregateRoot).Should().BeFalse();
			}
		}

		private static WeakReference<object> WeakAggregateRootFromReplay(PersistedEventStreamOpener<object, string> stream)
		{
			return new WeakReference<object>(stream.Replay(DummyAggregateRootId()));
		}
	}
}
