namespace Restall.Ichnaea
{
	public interface IDomainEventStream<TAggregateRoot> where TAggregateRoot: class
	{
		void CreateFrom(TAggregateRoot aggregateRoot);
		TAggregateRoot Replay(object aggregateRootId);
	}
}
