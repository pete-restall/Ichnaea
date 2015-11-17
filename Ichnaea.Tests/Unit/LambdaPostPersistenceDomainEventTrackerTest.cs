using System;
using FluentAssertions;
using NSubstitute;
using Restall.Ichnaea.Tests.Unit.Stubs;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class LambdaPostPersistenceDomainEventTrackerTest
	{
		[Fact]
		public void TrackToPersistentStore_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var tracker = new LambdaPostPersistenceDomainEventTracker<object>())
			{
				tracker.Invoking(x => x.TrackToPersistentStore(null, DummyPersistentObserver))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
			}
		}

		private static void DummyPersistentObserver(object aggregateRoot, object domainEvent)
		{
		}

		[Fact]
		public void TrackToPersistentStore_CalledWithNullPersistentObserver_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var tracker = new LambdaPostPersistenceDomainEventTracker<object>())
			{
				tracker.Invoking(x => x.TrackToPersistentStore(new object(), null))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("persistentObserver");
			}
		}

		[Fact]
		public void TrackToPersistentStore_Called_ExpectNewlySourcedDomainEventsAreTrackedToPersistentObserverInOrder()
		{
			using (var tracker = new LambdaPostPersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>())
			{
				var persistentObserver = Substitute.For<Source.Of<object>>();
				var aggregateRoot = CreateAggregateRootTrackedBy(tracker, persistentObserver);
				var domainEvents = new[] {new object(), new object()};
				aggregateRoot.SourceBothDomainEvents(domainEvents[0], domainEvents[1]);

				Received.InOrder(() =>
					{
						persistentObserver.Received(1)(aggregateRoot, domainEvents[0]);
						persistentObserver.Received(1)(aggregateRoot, domainEvents[1]);
					});
			}
		}

		private static AggregateRootWithTwoDomainEvents CreateAggregateRootTrackedBy(
			IPostPersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents> tracker, Source.Of<object> persistentObserver)
		{
			var aggregateRoot = new AggregateRootWithTwoDomainEvents();
			tracker.TrackToPersistentStore(aggregateRoot, persistentObserver);
			return aggregateRoot;
		}

		[Fact]
		public void TrackToPersistentStore_CalledWithAggregateRootAlreadyBeingTracked_ExpectAggregateRootAlreadyBeingTrackedException()
		{
			using (var tracker = new LambdaPostPersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>())
			{
				var tracked = CreateAggregateRootTrackedBy(tracker, DummyPersistentObserver);
				tracker.Invoking(x => x.TrackToPersistentStore(tracked, DummyPersistentObserver))
					.ShouldThrow<AggregateRootAlreadyBeingTrackedException>();
			}
		}

		[Fact]
		public void TrackToPersistentStore_CalledWithDifferentAggregateRoots_ExpectNewlySourcedDomainEventsAreTrackedToPersistentObserverInOrder()
		{
			using (var tracker = new LambdaPostPersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>())
			{
				var persistentObserver = Substitute.For<Source.Of<object>>();
				var aggregateRoots = new[]
					{
						CreateAggregateRootTrackedBy(tracker, persistentObserver),
						CreateAggregateRootTrackedBy(tracker, persistentObserver)
					};

				var domainEvents = new[] {new object(), new object()};
				aggregateRoots[0].SourceFirstDomainEvent(domainEvents[0]);
				aggregateRoots[1].SourceFirstDomainEvent(domainEvents[1]);

				Received.InOrder(() =>
				{
					persistentObserver.Received(1)(aggregateRoots[0], domainEvents[0]);
					persistentObserver.Received(1)(aggregateRoots[1], domainEvents[1]);
				});
			}
		}

		[Fact]
		public void TrackToPersistentStore_CalledAfterDisposeWithAggregateRootPreviouslyBeingTracked_ExpectNewlySourcedDomainEventsAreTrackedToPersistentObserver()
		{
			using (var tracker = new LambdaPostPersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>())
			{
				var persistentObserver = Substitute.For<Source.Of<object>>();
				var tracked = CreateAggregateRootTrackedBy(tracker, persistentObserver);
				tracker.Dispose();
				tracker.TrackToPersistentStore(tracked, persistentObserver);

				var domainEvent = new object();
				tracked.SourceFirstDomainEvent(domainEvent);
				persistentObserver.Received(1)(tracked, domainEvent);
			}
		}

		[Fact]
		public void Dispose_Called_ExpectPreviouslyTrackedAggregateRootsAreNotBeingSilentlyTrackedToPersistentStore()
		{
			using (var tracker = new LambdaPostPersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>())
			{
				var persistentObserver = Substitute.For<Source.Of<object>>();
				var aggregateRoots = new[]
					{
						CreateAggregateRootTrackedBy(tracker, persistentObserver),
						CreateAggregateRootTrackedBy(tracker, persistentObserver)
					};

				tracker.Dispose();
				aggregateRoots.ForEach(aggregateRoot => aggregateRoot.SourceFirstDomainEvent(new object()));
				persistentObserver.DidNotReceive()(Arg.Any<object>(), Arg.Any<object>());
			}
		}

		[Fact]
		public void ExpectGarbageCollectableAggregateRootsAreNotArtificiallyKeptAliveByTracking()
		{
			using (var tracker = new LambdaPostPersistenceDomainEventTracker<object>())
			{
				var weakAggregateRoot = WeakAggregateRootBeingTrackedBy(tracker);
				Collect.Garbage();

				object aggregateRoot;
				weakAggregateRoot.TryGetTarget(out aggregateRoot).Should().BeFalse();
			}
		}

		private static WeakReference<object> WeakAggregateRootBeingTrackedBy(IPostPersistenceDomainEventTracker<object> tracker)
		{
			return new WeakReference<object>(CreateAggregateRootTrackedBy(tracker));
		}

		private static AggregateRootWithTwoDomainEvents CreateAggregateRootTrackedBy(IPostPersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents> tracker)
		{
			return CreateAggregateRootTrackedBy(tracker, DummyPersistentObserver);
		}
	}
}
