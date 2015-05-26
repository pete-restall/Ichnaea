namespace Restall.Ichnaea
{
	public interface IReplayDomainEvents<TAggregateRoot> where TAggregateRoot: class
	{
		bool CanReplay(TAggregateRoot aggregateRoot, object domainEvent);
		TAggregateRoot Replay(TAggregateRoot aggregateRoot, object domainEvent);
	}
}
