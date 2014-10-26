using System;
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
			Action constructor = () => new DomainEventStream<object>(null, DummyPrePersistenceTracker<object>(), DummyIdGetter, DummyIdGetter);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventStore");
		}

		private static IPrePersistenceDomainEventTracker<T> DummyPrePersistenceTracker<T>()
		{
			return Substitute.For<IPrePersistenceDomainEventTracker<T>>();
		}

		private static string DummyIdGetter<T>(T aggregateRoot)
		{
			return string.Empty;
		}

		[Fact]
		public void Constructor_CalledWithNullPrePersistenceDomainEventTracker_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(DummyEventStore(), null, DummyIdGetter, DummyIdGetter);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("prePersistenceDomainEventTracker");
		}

		private static IStoreEvents DummyEventStore()
		{
			return Substitute.For<IStoreEvents>();
		}

		[Fact]
		public void Constructor_CalledWithNullAggregateRootIdGetter_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(DummyEventStore(), DummyPrePersistenceTracker<object>(), null, DummyIdGetter);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootIdGetter");
		}

		[Fact]
		public void Constructor_CalledWithNullBucketIdGetter_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStream<object>(DummyEventStore(), DummyPrePersistenceTracker<object>(), DummyIdGetter, null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("bucketIdGetter");
		}

		[Fact]
		public void CreateFrom_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var stream = new DomainEventStream<object>(DummyEventStore(), DummyPrePersistenceTracker<object>(), DummyIdGetter, DummyIdGetter);
			stream.Invoking(x => x.CreateFrom(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
		}

		[Fact]
		public void CreateFrom_Called_ExpectEventStreamIsCreatedWithIdFromAggregateRoot()
		{
			string bucketId = StringGenerator.AnyNonNull();
			string aggregateRootId = StringGenerator.AnyNonNull();
			var eventStore = Substitute.For<IStoreEvents>();
			eventStore.CreateStream(bucketId, aggregateRootId).Returns(Substitute.For<IEventStream>());

			var aggregateRoot = new object();
			var stream = new DomainEventStream<object>(
				eventStore,
				DummyPrePersistenceTracker<object>(),
				StubIdGetter(aggregateRoot, aggregateRootId),
				StubIdGetter(aggregateRoot, bucketId));

			stream.CreateFrom(aggregateRoot);
			eventStore.Received(1).CreateStream(bucketId, aggregateRootId);
		}

		private static Func<object, string> StubIdGetter(object aggregateRoot, string bucketId)
		{
			var bucketIdGetter = Substitute.For<Func<object, string>>();
			bucketIdGetter(aggregateRoot).Returns(bucketId);
			return bucketIdGetter;
		}

		[Fact]
		public void CreateFrom_Called_ExpectPrePersistenceTrackerIsSwitchedToTheEventStoreForTheAggregateRoot()
		{
			var prePersistenceTracker = Substitute.For<IPrePersistenceDomainEventTracker<object>>();
			var stream = new DomainEventStream<object>(
				DummyEventStore(),
				prePersistenceTracker,
				DummyIdGetter,
				DummyIdGetter);

			var aggregateRoot = new object();
			stream.CreateFrom(aggregateRoot);

			prePersistenceTracker.Received(1).SwitchTrackingToPersistentStore(aggregateRoot, Arg.Is<Source.Of<object>>(x => x != null));
		}

		[Fact]
		public void Replay_CalledWithNullId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var stream = new DomainEventStream<object>(DummyEventStore(), DummyPrePersistenceTracker<object>(), DummyIdGetter, DummyIdGetter);
			stream.Invoking(x => x.Replay(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("id");
		}

		// TODO: CreateFrom() - error conditions for SwitchTrackingToPersistentStore() - should the stream stay created and empty or be rolled back (prolly not possible to roll back if events half persisted) ?
		// TODO: CreateFrom() - when NEventStore throws exception, should not lose Domain Events...(ie. should be able to try again)
		// TODO: CreateFrom() - Disposal of stream (eventually...)
		// TODO: CreateFrom() - Future events are persisted to the store, not the previous in-memory tracker
	}
}
