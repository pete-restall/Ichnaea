namespace Restall.Ichnaea
{
	public interface IPostPersistenceDomainEventTracker<in TAggregateRoot>
		where TAggregateRoot: class
	{
		void TrackToPersistentStore(TAggregateRoot aggregateRoot, Source.Of<object> persistentObserver);
	}
}
