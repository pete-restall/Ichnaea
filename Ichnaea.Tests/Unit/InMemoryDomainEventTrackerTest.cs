using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Restall.Ichnaea.Tests.Unit.Stubs;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class InMemoryDomainEventTrackerTest
	{
		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullPostPersistenceDomainEventTracker_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new InMemoryDomainEventTracker<object>(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("postPersistenceDomainEventTracker");
		}

		[Fact]
		public void AggregateRootCreated_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<object>())
			{
				tracker.Invoking(x => x.AggregateRootCreated(null, new object()))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
			}
		}

		private static InMemoryDomainEventTracker<TAggregateRoot> CreateTrackerWithDummyDependencies<TAggregateRoot>() where TAggregateRoot: class
		{
			return new InMemoryDomainEventTracker<TAggregateRoot>(PostPersistenceDomainEventTrackerTestDoubles.Dummy());
		}

		[Fact]
		public void AggregateRootCreated_CalledWithNullDomainEvent_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<object>())
			{
				tracker.Invoking(x => x.AggregateRootCreated(new object(), null))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("domainEvent");
			}
		}

		[Fact]
		public void AggregateRootCreated_CalledWithAggregateRootAlreadyBeingTracked_ExpectAggregateRootAlreadyBeingTrackedException()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				var aggregateRoot = CreateAggregateRootTrackedBy(tracker);
				tracker.Invoking(x => x.AggregateRootCreated(aggregateRoot, new object())).ShouldThrow<AggregateRootAlreadyBeingTrackedException>();
			}
		}

		private static AggregateRootWithTwoDomainEvents CreateAggregateRootTrackedBy(IDomainEventTracker<AggregateRootWithTwoDomainEvents> tracker)
		{
			var aggregateRoot = new AggregateRootWithTwoDomainEvents();
			tracker.AggregateRootCreated(aggregateRoot, new object());
			return aggregateRoot;
		}

		[Fact]
		public void GetSourcedDomainEventsFor_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<object>())
			{
				tracker.Invoking(x => x.GetSourcedDomainEventsFor(null))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
			}
		}

		[Fact]
		public void GetSourcedDomainEventsFor_CalledWithNewlyCreatedAggregateRoot_ExpectCreationDomainEventIsReturned()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				var newlyCreated = new AggregateRootWithTwoDomainEvents();
				var creationEvent = new object();
				tracker.AggregateRootCreated(newlyCreated, creationEvent);
				tracker.GetSourcedDomainEventsFor(newlyCreated).Single().Should().BeSameAs(creationEvent);
			}
		}

		[Fact]
		public void GetSourcedDomainEventsFor_CalledWithUntrackedAggregateRoot_ExpectEmptyEnumerableIsReturned()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				var untracked = new AggregateRootWithTwoDomainEvents();
				tracker.GetSourcedDomainEventsFor(untracked).Should().BeEmpty();
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = CodeAnalysisJustification.EnumerableIsMaterialisedBeforeDisposal)]
		public void GetSourcedDomainEventsFor_CalledWithOneOfSeveralTrackedAggregateRoots_ExpectCorrespondingCreationDomainEventIsReturned()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				int numberOfTracked = IntegerGenerator.WithinExclusiveRange(1, 10);
				var tracked = numberOfTracked.Select(() => new AggregateRootWithTwoDomainEvents()).ToArray();
				var creationEvents = numberOfTracked.Select(() => new object()).ToArray();
				numberOfTracked.Repeat(i => tracker.AggregateRootCreated(tracked[i], creationEvents[i]));

				int anyTrackedIndex = IntegerGenerator.WithinExclusiveRange(0, numberOfTracked);
				tracker.GetSourcedDomainEventsFor(tracked[anyTrackedIndex])
					.Single().Should().BeSameAs(creationEvents[anyTrackedIndex]);
			}
		}

		[Fact]
		public void GetSourcedDomainEventsFor_CalledWhenTrackedAggregateRootSourcesDomainEvents_ExpectAllSourcedDomainEventsAreReturnedInOrder()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				var aggregateRoot = new AggregateRootWithTwoDomainEvents();
				var creationEvent = new object();
				tracker.AggregateRootCreated(aggregateRoot, creationEvent);

				var firstEvent = new object();
				var secondEvent = new object();
				aggregateRoot.SourceBothDomainEvents(firstEvent, secondEvent);
				tracker.GetSourcedDomainEventsFor(aggregateRoot).Should().Equal(creationEvent, firstEvent, secondEvent);
			}
		}

		[Fact]
		public void GetSourcedDomainEventsFor_CalledAfterDisposeWithPreviouslyTrackedAggregateRoot_ExpectEmptyEnumerableIsReturned()
		{
			var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>();
			var aggregateRoot = CreateAggregateRootTrackedBy(tracker);
			tracker.Dispose();
			tracker.GetSourcedDomainEventsFor(aggregateRoot).Should().BeEmpty();
		}

		[Fact]
		public void SwitchTrackingToPersistentStore_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				((IPrePersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>) tracker)
					.Invoking(x => x.SwitchTrackingToPersistentStore(null, DummyPersistentObserver))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
			}
		}

		private static void DummyPersistentObserver(object aggregateRoot, object domainEvent)
		{
		}

		[Fact]
		public void SwitchTrackingToPersistentStore_CalledWithNullPersistentObserver_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				((IPrePersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>) tracker)
					.Invoking(x => x.SwitchTrackingToPersistentStore(new AggregateRootWithTwoDomainEvents(), null))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("persistentObserver");
			}
		}

		[Fact]
		public void SwitchTrackingToPersistentStore_CalledWithUntrackedAggregateRoot_ExpectAggregateRootNotBeingTrackedException()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				((IPrePersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>) tracker)
					.Invoking(x => x.SwitchTrackingToPersistentStore(new AggregateRootWithTwoDomainEvents(), DummyPersistentObserver))
					.ShouldThrow<AggregateRootNotBeingTrackedException>();
			}
		}

		[Fact]
		public void SwitchTrackingToPersistentStore_CalledTwiceWithAggregateRoot_ExpectAggregateRootNotBeingTrackedException()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				var aggregateRoot = CreateAggregateRootTrackedBy(tracker);
				var prePersistenceTracker = (IPrePersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>) tracker;
				prePersistenceTracker.SwitchTrackingToPersistentStore(aggregateRoot, DummyPersistentObserver);

				prePersistenceTracker
					.Invoking(x => x.SwitchTrackingToPersistentStore(aggregateRoot, DummyPersistentObserver))
					.ShouldThrow<AggregateRootNotBeingTrackedException>();
			}
		}

		[Fact]
		public void SwitchTrackingToPersistentStore_Called_ExpectNewlySourcedDomainEventsAreNotTrackedInMemory()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				var aggregateRoot = CreateAggregateRootTrackedBy(tracker);
				var inMemoryDomainEvents = tracker.GetSourcedDomainEventsFor(aggregateRoot);
				((IPrePersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>) tracker).SwitchTrackingToPersistentStore(aggregateRoot, DummyPersistentObserver);

				var newlySourcedDomainEvent = new object();
				aggregateRoot.SourceFirstDomainEvent(newlySourcedDomainEvent);
				inMemoryDomainEvents.Should().NotContain(newlySourcedDomainEvent);
			}
		}

		[Fact]
		public void SwitchTrackingToPersistentStore_Called_ExpectPersistentObserverIsCalledWithPreviouslySourcedDomainEventsInOrder()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				var aggregateRoot = CreateAggregateRootTrackedBy(tracker);
				int numberOfDomainEvents = IntegerGenerator.WithinExclusiveRange(0, 10);
				numberOfDomainEvents.Repeat(() => aggregateRoot.SourceFirstDomainEvent(new object()));

				var persistentObserver = Substitute.For<Source.Of<object>>();
				var previouslySourcedDomainEvents = tracker.GetSourcedDomainEventsFor(aggregateRoot).ToArray();
				((IPrePersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>) tracker).SwitchTrackingToPersistentStore(aggregateRoot, persistentObserver);

				Received.InOrder(() =>
					previouslySourcedDomainEvents.ForEach(domainEvent => persistentObserver.Received(1)(aggregateRoot, domainEvent)));
			}
		}

		[Fact]
		public void SwitchTrackingToPersistentStore_Called_ExpectPostPersistenceDomainEventTrackerIsGivenTheAggregateRootAndObservable()
		{
			var postPersistenceDomainEventTracker = Substitute.For<IPostPersistenceDomainEventTracker<object>>();
			using (var tracker = new InMemoryDomainEventTracker<AggregateRootWithTwoDomainEvents>(postPersistenceDomainEventTracker))
			{
				var aggregateRoot = CreateAggregateRootTrackedBy(tracker);
				Source.Of<object> persistentObserver = DummyPersistentObserver;
				((IPrePersistenceDomainEventTracker<AggregateRootWithTwoDomainEvents>) tracker).SwitchTrackingToPersistentStore(aggregateRoot, persistentObserver);

				postPersistenceDomainEventTracker.Received(1).TrackToPersistentStore(aggregateRoot, persistentObserver);
			}
		}

		[Fact]
		public void Dispose_Called_ExpectPreviouslyTrackedAggregateRootsAreNotBeingSilentlyTrackedToPreviousDomainEventCollection()
		{
			using (var tracker = CreateTrackerWithDummyDependencies<AggregateRootWithTwoDomainEvents>())
			{
				var aggregateRoot = CreateAggregateRootTrackedBy(tracker);
				var domainEvents = (ICollection<object>) tracker.GetSourcedDomainEventsFor(aggregateRoot);
				int countOfDomainEventsPriorToDispose = domainEvents.Count;
				tracker.Dispose();
				aggregateRoot.SourceFirstDomainEvent(new object());
				domainEvents.Count.Should().Be(countOfDomainEventsPriorToDispose);
			}
		}

		[Fact]
		public void ExpectGarbageCollectableAggregateRootsAreNotArtificiallyKeptAliveByTracking()
		{
			ExpectGarbageCollectableAggregateRootsAreNotArtificiallyKeptAliveBy(WeakAggregateRootFromCreatedEvent);
		}

		private static void ExpectGarbageCollectableAggregateRootsAreNotArtificiallyKeptAliveBy(Func<InMemoryDomainEventTracker<object>, WeakReference<object>> action)
		{
			using (var tracker = CreateTrackerWithDummyDependencies<object>())
			{
				var weakAggregateRoot = action(tracker);
				Collect.Garbage();

				object aggregateRoot;
				weakAggregateRoot.TryGetTarget(out aggregateRoot).Should().BeFalse();
			}
		}

		private static WeakReference<object> WeakAggregateRootFromCreatedEvent(IDomainEventTracker<object> tracker)
		{
			return new WeakReference<object>(CreateAggregateRootTrackedBy(tracker));
		}

		[Fact]
		public void ExpectGarbageCollectableAggregateRootsAreNotArtificiallyKeptAliveAfterSwitchingToPersistentStore()
		{
			ExpectGarbageCollectableAggregateRootsAreNotArtificiallyKeptAliveBy(WeakAggregateRootFromSwitchToPersistentStore);
		}

		private static WeakReference<object> WeakAggregateRootFromSwitchToPersistentStore(InMemoryDomainEventTracker<object> tracker)
		{
			var aggregateRoot = CreateAggregateRootTrackedBy(tracker);
			((IPrePersistenceDomainEventTracker<object>) tracker).SwitchTrackingToPersistentStore(aggregateRoot, DummyPersistentObserver);
			return new WeakReference<object>(aggregateRoot);
		}
	}
}
