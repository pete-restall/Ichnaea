using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Restall.Ichnaea
{
	public class InMemoryDomainEventTracker<TAggregateRoot>: DisposableContainer, IDomainEventTracker<TAggregateRoot>, IPrePersistenceDomainEventTracker<TAggregateRoot>
		where TAggregateRoot: class
	{
		private class TrackingInfo
		{
			public TrackingInfo(List<object> domainEvents, DomainEventFunnel funnel)
			{
				this.DomainEvents = domainEvents;
				this.Funnel = funnel;
			}

			public List<object> DomainEvents { get; }

			public DomainEventFunnel Funnel { get; }
		}

		private readonly IPostPersistenceDomainEventTracker<TAggregateRoot> postPersistenceDomainEventTracker;

		private ConditionalWeakTable<TAggregateRoot, TrackingInfo> aggregateToDomainEventsMap =
			new ConditionalWeakTable<TAggregateRoot, TrackingInfo>();

		public InMemoryDomainEventTracker(IPostPersistenceDomainEventTracker<TAggregateRoot> postPersistenceDomainEventTracker)
		{
			this.postPersistenceDomainEventTracker = postPersistenceDomainEventTracker;
		}

		public void AggregateRootCreated(TAggregateRoot aggregateRoot, object domainEvent)
		{
			TrackingInfo trackingInfo;
			if (this.aggregateToDomainEventsMap.TryGetValue(aggregateRoot, out trackingInfo))
				throw new AggregateRootAlreadyBeingTrackedException();

			var domainEvents = new List<object> {domainEvent};
			var funnel = this.NewFunnelFor(aggregateRoot, (sender, args) => domainEvents.Add(args));
			this.aggregateToDomainEventsMap.Add(aggregateRoot, new TrackingInfo(domainEvents, funnel));
		}

		private DomainEventFunnel NewFunnelFor(TAggregateRoot aggregateRoot, Source.Of<object> observer)
		{
			var funnel = new DomainEventFunnel(aggregateRoot, observer);
			this.AddDisposable(funnel);
			return funnel;
		}

		public IEnumerable<object> GetSourcedDomainEventsFor(TAggregateRoot aggregateRoot)
		{
			TrackingInfo trackingInfo;
			if (this.aggregateToDomainEventsMap.TryGetValue(aggregateRoot, out trackingInfo))
				return trackingInfo.DomainEvents;

			return new object[0];
		}

		void IPrePersistenceDomainEventTracker<TAggregateRoot>.SwitchTrackingToPersistentStore(
			TAggregateRoot aggregateRoot, Source.Of<object> persistentObserver)
		{
			TrackingInfo trackingInfo;
			if (!this.aggregateToDomainEventsMap.TryGetValue(aggregateRoot, out trackingInfo))
				throw new AggregateRootNotBeingTrackedException();

			trackingInfo.DomainEvents.ForEach(domainEvent => persistentObserver(aggregateRoot, domainEvent));
			this.aggregateToDomainEventsMap.Remove(aggregateRoot);
			this.RemoveDisposable(trackingInfo.Funnel);
			trackingInfo.Funnel.Dispose();

			this.postPersistenceDomainEventTracker.TrackToPersistentStore(aggregateRoot, persistentObserver);
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			this.aggregateToDomainEventsMap = new ConditionalWeakTable<TAggregateRoot, TrackingInfo>();
			base.Dispose(true);
		}
	}
}
