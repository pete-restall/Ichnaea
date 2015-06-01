namespace Restall.Ichnaea.NEventStore
{
	public class DomainEventStreamAdapter<TAggregateRoot>: IDomainEventStream<TAggregateRoot>
		where TAggregateRoot: class
	{
		private readonly PersistedEventStreamCreator<TAggregateRoot> persistedEventStreamCreator;
		private readonly PersistedEventStreamOpener<TAggregateRoot> persistedEventStreamOpener;

		public DomainEventStreamAdapter(
			PersistedEventStreamCreator<TAggregateRoot> persistedEventStreamCreator,
			PersistedEventStreamOpener<TAggregateRoot> persistedEventStreamOpener)
		{
			this.persistedEventStreamCreator = persistedEventStreamCreator;
			this.persistedEventStreamOpener = persistedEventStreamOpener;
		}

		public void CreateFrom(TAggregateRoot aggregateRoot)
		{
			this.persistedEventStreamCreator.CreateFrom(aggregateRoot);
		}

		public TAggregateRoot Replay(string aggregateRootId)
		{
			return this.persistedEventStreamOpener.Replay(aggregateRootId);
		}
	}
}
