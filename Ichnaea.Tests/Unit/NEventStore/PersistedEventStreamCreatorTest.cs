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
			Action constructor = () => new PersistedEventStreamCreator<object>(
				null,
				DummyBucketId(),
				PrePersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyAggregateRootIdGetter,
				DummyEventConverter);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventStore");
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
		public void Constructor_CalledWithNullBucketId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventStreamCreator<object>(
				DummyEventStore(),
				null,
				PrePersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyAggregateRootIdGetter,
				DummyEventConverter);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("bucketId");
		}

		private static IStoreEvents DummyEventStore()
		{
			return Substitute.For<IStoreEvents>();
		}

		[Fact]
		public void Constructor_CalledWithNullPrePersistenceDomainEventTracker_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventStreamCreator<object>(
				DummyEventStore(),
				DummyBucketId(),
				null,
				DummyAggregateRootIdGetter,
				DummyEventConverter);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("prePersistenceDomainEventTracker");
		}

		[Fact]
		public void Constructor_CalledWithNullAggregateRootIdGetter_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventStreamCreator<object>(
				DummyEventStore(),
				DummyBucketId(),
				PrePersistenceDomainEventTrackerTestDoubles.Dummy(),
				null,
				DummyEventConverter);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootIdGetter");
		}

		[Fact]
		public void Constructor_CalledWithNullDomainEventToPersistedEventConverter_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventStreamCreator<object>(
				DummyEventStore(),
				DummyBucketId(),
				PrePersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyAggregateRootIdGetter,
				null);

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("domainEventToPersistedEventConverter");
		}

		[Fact]
		public void CreateFrom_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var stream = new PersistedEventStreamCreator<object>(
				DummyEventStore(),
				DummyBucketId(),
				PrePersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyAggregateRootIdGetter,
				DummyEventConverter))
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
			using (var stream = new PersistedEventStreamCreator<object>(
				eventStore,
				bucketId,
				PrePersistenceDomainEventTrackerTestDoubles.Dummy(),
				StubAggregateRootIdGetter(aggregateRoot, aggregateRootId),
				DummyEventConverter))
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
			var prePersistenceTracker = PrePersistenceDomainEventTrackerTestDoubles.Mock();
			using (var stream = new PersistedEventStreamCreator<object>(
				DummyEventStore(),
				DummyBucketId(),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyEventConverter))
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
			var persistedEvents = domainEvents.Select(x => new EventMessage()).ToArray();

			string bucketId = DummyBucketId();
			string aggregateRootId = DummyAggregateRootId();
			var eventStoreStream = Substitute.For<IEventStream>();
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.CreateStream(bucketId, aggregateRootId).Returns(eventStoreStream);

			var aggregateRoot = new object();
			using (var stream = new PersistedEventStreamCreator<object>(
				eventStore,
				bucketId,
				StubPrePersistenceDomainEventTracker(aggregateRoot, domainEvents),
				StubAggregateRootIdGetter(aggregateRoot, aggregateRootId),
				StubEventConverter(domainEvents, persistedEvents)))
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
			var prePersistenceTracker = PrePersistenceDomainEventTrackerTestDoubles.Stub();
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
			var prePersistenceTracker = PrePersistenceDomainEventTrackerTestDoubles.Spy();
			prePersistenceTracker.SwitchTrackingToPersistentStore(aggregateRoot, Arg.Do<Source.Of<object>>(x => capturedSourceEvent = x));

			var eventStoreStream = Substitute.For<IEventStream>();
			using (var stream = new PersistedEventStreamCreator<object>(
				StubEventStoreCreateStream(eventStoreStream),
				DummyBucketId(),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				StubEventConverter(domainEvents, persistedEvents)))
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
			var prePersistenceTracker = PrePersistenceDomainEventTrackerTestDoubles.Mock();
			prePersistenceTracker.When(x => x.SwitchTrackingToPersistentStore(Arg.Any<object>(), Arg.Any<Source.Of<object>>()))
				.Do(x => { throw new Exception(); });

			var eventStoreStream = MockEventStoreStream();
			using (var stream = new PersistedEventStreamCreator<object>(
				StubEventStoreCreateStream(eventStoreStream),
				DummyBucketId(),
				prePersistenceTracker,
				DummyAggregateRootIdGetter,
				DummyEventConverter))
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
			using (var stream = new PersistedEventStreamCreator<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyBucketId(),
				StubPrePersistenceDomainEventTrackerToSourceOneDomainEventPerAggregateRoot(aggregateRoots, domainEvents),
				DummyAggregateRootIdGetter,
				x => (EventMessage) x))
			{
				aggregateRoots.ForEach(aggregateRoot => stream.CreateFrom(aggregateRoot));
				numberOfAggregateRoots.Repeat(i => eventStoreStreams[i].Received(1).Add(domainEvents[i]));
			}
		}

		private static IPrePersistenceDomainEventTracker<object> StubPrePersistenceDomainEventTrackerToSourceOneDomainEventPerAggregateRoot(
			IList<object> aggregateRoots, IList<object> domainEvents)
		{
			var prePersistenceDomainEventTracker = PrePersistenceDomainEventTrackerTestDoubles.Stub();
			aggregateRoots.Count.Repeat(i =>
				prePersistenceDomainEventTracker.SwitchTrackingToPersistentStore(
					aggregateRoots[i],
					Arg.Do<Source.Of<object>>(eventSource => eventSource(aggregateRoots[i], domainEvents[i]))));

			return prePersistenceDomainEventTracker;
		}

		[Fact]
		public void Dispose_Called_ExpectCreatedEventStoreStreamsAreDisposed()
		{
			var eventStoreStreams = MockAtLeastOneEventStoreStream();
			using (var stream = new PersistedEventStreamCreator<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyBucketId(),
				PrePersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyAggregateRootIdGetter,
				DummyEventConverter))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
			}

			eventStoreStreams.ForEach(stream => stream.Received(1).Dispose());
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
		public void Dispose_CalledMultipleTimes_ExpectCreatedEventStoreStreamsAreNotDisposedMoreThanOnce()
		{
			var eventStoreStreams = MockAtLeastOneEventStoreStream();
			using (var stream = new PersistedEventStreamCreator<object>(
				StubEventStoreCreateStream(eventStoreStreams),
				DummyBucketId(),
				PrePersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyAggregateRootIdGetter,
				DummyEventConverter))
			{
				eventStoreStreams.ForEach(x => stream.CreateFrom(new object()));
				stream.Dispose();
			}

			eventStoreStreams.ForEach(stream => stream.Received(1).Dispose());
		}

		[Fact]
		public void ExpectGarbageCollectableAggregateRootsAreNotArtificiallyKeptAliveByTracking()
		{
			using (var stream = new PersistedEventStreamCreator<object>(
				DummyEventStore(),
				DummyBucketId(),
				PrePersistenceDomainEventTrackerTestDoubles.Dummy(),
				DummyAggregateRootIdGetter,
				DummyEventConverter))
			{
				var weakAggregateRoot = WeakAggregateRootFromCreate(stream);
				Collect.Garbage();

				object aggregateRoot;
				weakAggregateRoot.TryGetTarget(out aggregateRoot).Should().BeFalse();
			}
		}

		private static WeakReference<object> WeakAggregateRootFromCreate(PersistedEventStreamCreator<object> stream)
		{
			var aggregateRoot = new object();
			stream.CreateFrom(aggregateRoot);
			return new WeakReference<object>(aggregateRoot);
		}
	}
}
