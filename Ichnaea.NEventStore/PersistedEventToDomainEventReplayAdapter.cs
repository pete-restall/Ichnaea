using System;
using NEventStore;
using NullGuard;

namespace Restall.Ichnaea.NEventStore
{
	public class PersistedEventToDomainEventReplayAdapter<TAggregateRoot> where TAggregateRoot: class
	{
		private readonly Converter<EventMessage, object> persistedEventToDomainEventConverter;
		private readonly IReplayDomainEvents<TAggregateRoot> domainEventReplay;

		public PersistedEventToDomainEventReplayAdapter(
			Converter<EventMessage, object> persistedEventToDomainEventConverter,
			IReplayDomainEvents<TAggregateRoot> domainEventReplay)
		{
			this.persistedEventToDomainEventConverter = persistedEventToDomainEventConverter;
			this.domainEventReplay = domainEventReplay;
		}

		public virtual bool CanReplay([AllowNull] TAggregateRoot aggregateRoot, EventMessage persistedEvent)
		{
			return this.domainEventReplay.CanReplay(aggregateRoot, this.persistedEventToDomainEventConverter(persistedEvent));
		}

		public virtual TAggregateRoot Replay([AllowNull] TAggregateRoot aggregateRoot, EventMessage persistedEvent)
		{
			return this.domainEventReplay.Replay(aggregateRoot, this.persistedEventToDomainEventConverter(persistedEvent));
		}
	}
}
