namespace Restall.Ichnaea
{
	public interface IPrePersistenceDomainEventTracker<in TAggregateRoot>: IDomainEventTracker<TAggregateRoot>
	{
		void SwitchTrackingToPersistentStore(TAggregateRoot aggregateRoot, Source.Of<object> persistentObserver);
	}
}
