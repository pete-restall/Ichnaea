using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class InMemoryDomainEventTrackerTest
	{
		private class StubAggregateRoot
		{
			public void SourceFirstDomainEvent(object domainEvent)
			{
				this.SourceEvent(this.FirstEvent, domainEvent);
			}

			private void SourceEvent(Source.Of<object> eventField, object domainEvent)
			{
				eventField?.Invoke(this, domainEvent);
			}

			public void SourceBothDomainEvents(object firstDomainEvent, object secondDomainEvent)
			{
				this.SourceEvent(this.FirstEvent, firstDomainEvent);
				this.SourceEvent(this.SecondEvent, secondDomainEvent);
			}

			public event Source.Of<object> FirstEvent;
			public event Source.Of<object> SecondEvent;
		}

		[Fact]
		public void AggregateRootCreated_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var tracker = new InMemoryDomainEventTracker<object>())
			{
				tracker.Invoking(x => x.AggregateRootCreated(null, new object()))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
			}
		}

		[Fact]
		public void AggregateRootCreated_CalledWithNullDomainEvent_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var tracker = new InMemoryDomainEventTracker<object>())
			{
				tracker.Invoking(x => x.AggregateRootCreated(new object(), null))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("domainEvent");
			}
		}

		[Fact]
		public void AggregateRootCreated_CalledWithAggregateRootAlreadyBeingTracked_ExpectAggregateRootAlreadyBeingTrackedException()
		{
			using (var tracker = new InMemoryDomainEventTracker<StubAggregateRoot>())
			{
				var aggregateRoot = new StubAggregateRoot();
				tracker.AggregateRootCreated(aggregateRoot, new object());
				tracker.Invoking(x => x.AggregateRootCreated(aggregateRoot, new object())).ShouldThrow<AggregateRootAlreadyBeingTrackedException>();
			}
		}

		[Fact]
		public void GetSourcedDomainEventsFor_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var tracker = new InMemoryDomainEventTracker<object>())
			{
				tracker.Invoking(x => x.GetSourcedDomainEventsFor(null))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
			}
		}

		[Fact]
		public void GetSourcedDomainEventsFor_CalledWithNewlyCreatedAggregateRoot_ExpectCreationDomainEventIsReturned()
		{
			using (var tracker = new InMemoryDomainEventTracker<StubAggregateRoot>())
			{
				var newlyCreated = new StubAggregateRoot();
				var creationEvent = new object();
				tracker.AggregateRootCreated(newlyCreated, creationEvent);
				tracker.GetSourcedDomainEventsFor(newlyCreated).Single().Should().BeSameAs(creationEvent);
			}
		}

		[Fact]
		public void GetSourcedDomainEventsFor_CalledWithUntrackedAggregateRoot_ExpectEmptyEnumerableIsReturned()
		{
			using (var tracker = new InMemoryDomainEventTracker<StubAggregateRoot>())
			{
				var untracked = new StubAggregateRoot();
				tracker.GetSourcedDomainEventsFor(untracked).Should().BeEmpty();
			}
		}

		[Fact]
		public void GetSourcedDomainEventsFor_CalledWithOneOfSeveralTrackedAggregateRoots_ExpectCorrespondingCreationDomainEventIsReturned()
		{
			using (var tracker = new InMemoryDomainEventTracker<StubAggregateRoot>())
			{
				int numberOfTracked = IntegerGenerator.WithinExclusiveRange(1, 10);
				var tracked = numberOfTracked.Select(() => new StubAggregateRoot()).ToArray();
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
			using (var tracker = new InMemoryDomainEventTracker<StubAggregateRoot>())
			{
				var aggregateRoot = new StubAggregateRoot();
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
			var tracked = new StubAggregateRoot();
			var tracker = new InMemoryDomainEventTracker<StubAggregateRoot>();
			tracker.AggregateRootCreated(tracked, new object());
			tracker.Dispose();
			tracker.GetSourcedDomainEventsFor(tracked).Should().BeEmpty();
		}

		[Fact]
		public void Dispose_Called_ExpectPreviouslyTrackedAggregateRootsAreNotBeingSilentlyTrackedToPreviousDomainEventCollection()
		{
			using (var tracker = new InMemoryDomainEventTracker<StubAggregateRoot>())
			{
				var aggregateRoot = new StubAggregateRoot();
				tracker.AggregateRootCreated(aggregateRoot, new object());
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
			using (var tracker = new InMemoryDomainEventTracker<object>())
			{
				var weakAggregateRoot = WeakAggregateRootFromCreatedEvent(tracker);
				Collect.Garbage();

				object aggregateRoot;
				weakAggregateRoot.TryGetTarget(out aggregateRoot).Should().BeFalse();
			}
		}

		private static WeakReference<object> WeakAggregateRootFromCreatedEvent(InMemoryDomainEventTracker<object> tracker)
		{
			object aggregateRoot = new object();
			tracker.AggregateRootCreated(aggregateRoot, new object());
			return new WeakReference<object>(aggregateRoot);
		}
	}
}
