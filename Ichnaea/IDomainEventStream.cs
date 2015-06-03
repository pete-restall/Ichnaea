namespace Restall.Ichnaea
{
	public interface IDomainEventStream<TAggregateRoot, in TAggregateRootId> where TAggregateRoot: class
	{
		void CreateFrom(TAggregateRoot aggregateRoot);
		TAggregateRoot Replay(TAggregateRootId aggregateRootId);
	}
}
