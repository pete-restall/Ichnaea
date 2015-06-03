namespace Restall.Ichnaea.NEventStore
{
	public class DomainEventStreamAdapter<TAggregateRoot, TAggregateRootId>: IDomainEventStream<TAggregateRoot, TAggregateRootId>
		where TAggregateRoot: class
	{
		private readonly PersistedEventStreamCreator<TAggregateRoot> persistedEventStreamCreator;
		private readonly PersistedEventStreamOpener<TAggregateRoot, TAggregateRootId> persistedEventStreamOpener;

		public DomainEventStreamAdapter(
			PersistedEventStreamCreator<TAggregateRoot> persistedEventStreamCreator,
			PersistedEventStreamOpener<TAggregateRoot, TAggregateRootId> persistedEventStreamOpener)
		{
			this.persistedEventStreamCreator = persistedEventStreamCreator;
			this.persistedEventStreamOpener = persistedEventStreamOpener;
		}

		public void CreateFrom(TAggregateRoot aggregateRoot)
		{
			this.persistedEventStreamCreator.CreateFrom(aggregateRoot);
		}

		public TAggregateRoot Replay(TAggregateRootId aggregateRootId)
		{
			return this.persistedEventStreamOpener.Replay(aggregateRootId);
		}
	}
}
