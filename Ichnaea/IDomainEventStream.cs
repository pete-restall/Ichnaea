namespace Restall.Ichnaea
{
	public interface IDomainEventStream<TAggregateRoot>
	{
		void CreateFrom(TAggregateRoot root);
		TAggregateRoot Replay(string id);
	}
}
