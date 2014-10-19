namespace Restall.Ichnaea
{
	public interface IDomainEventTracker<in T>
	{
		void AggregateRootCreated(T aggregateRoot, object domainEvent);
	}
}
