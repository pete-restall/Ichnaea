namespace Restall.Ichnaea
{
	public interface IPrePersistenceDomainEventTracker<in TAggregateRoot>
		where TAggregateRoot: class
	{
		void SwitchTrackingToPersistentStore(TAggregateRoot aggregateRoot, Source.Of<object> persistentObserver);
	}
}
