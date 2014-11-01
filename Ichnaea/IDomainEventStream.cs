namespace Restall.Ichnaea
{
	public interface IDomainEventStream<TAggregateRoot>
	{
		void CreateFrom(TAggregateRoot aggregateRoot);
		TAggregateRoot Replay(string aggregateRootId);
	}
}
