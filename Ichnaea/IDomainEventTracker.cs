namespace Restall.Ichnaea
{
	public interface IDomainEventTracker<in TAggregateRoot> where TAggregateRoot: class
	{
		void AggregateRootCreated(TAggregateRoot aggregateRoot, object domainEvent);
	}
}
