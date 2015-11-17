using System.Runtime.CompilerServices;

namespace Restall.Ichnaea
{
	public class LambdaPostPersistenceDomainEventTracker<TAggregateRoot>: DisposableContainer, IPostPersistenceDomainEventTracker<TAggregateRoot>
		where TAggregateRoot: class
	{
		private ConditionalWeakTable<TAggregateRoot, DomainEventFunnel> trackedAggregateRoots =
			new ConditionalWeakTable<TAggregateRoot, DomainEventFunnel>();

		public void TrackToPersistentStore(TAggregateRoot aggregateRoot, Source.Of<object> persistentObserver)
		{
			DomainEventFunnel funnel;
			if (this.trackedAggregateRoots.TryGetValue(aggregateRoot, out funnel))
				throw new AggregateRootAlreadyBeingTrackedException();

			this.trackedAggregateRoots.Add(aggregateRoot, this.NewFunnelFor(aggregateRoot, persistentObserver));
		}

		private DomainEventFunnel NewFunnelFor(TAggregateRoot aggregateRoot, Source.Of<object> observer)
		{
			var funnel = new DomainEventFunnel(aggregateRoot, observer);
			this.AddDisposable(funnel);
			return funnel;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				this.trackedAggregateRoots = new ConditionalWeakTable<TAggregateRoot, DomainEventFunnel>();

			base.Dispose(disposing);
		}
	}
}
