namespace Restall.Ichnaea
{
	public interface IDomainEventTracker<in TAggregateRoot>
	{
		void AggregateRootCreated(TAggregateRoot aggregateRoot, object domainEvent);
	}
}
