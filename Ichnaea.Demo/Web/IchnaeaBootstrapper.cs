using Nancy.TinyIoc;
using NEventStore;
using Restall.Ichnaea.Demo.Accounts;
using Restall.Ichnaea.NEventStore;

namespace Restall.Ichnaea.Demo.Web
{
	public static class IchnaeaBootstrapper
	{
		public static void RegisterRequestScopeIchnaeaDependenciesInto(TinyIoCContainer container)
		{
			// TODO: PROPER FLUENT / CONVENTION-BASED CONFIGURATION
			container.Register((ctx, _) => CreateDomainEventStream(ctx.Resolve<NEventStoreSession>(), new NamedPropertyAggregateRootIdGetter<Account, AccountId>("Id")));
		}

		private static IDomainEventStream<TAggregateRoot, TAggregateRootId> CreateDomainEventStream<TAggregateRoot, TAggregateRootId>(
			IStoreEvents session, IAggregateRootIdGetter<TAggregateRoot, TAggregateRootId> idGetter)
			where TAggregateRoot: class
		{
			var postPersistenceDomainEventTracker = new LambdaPostPersistenceDomainEventTracker<TAggregateRoot>();
			string bucketId = typeof(TAggregateRoot).Name;
			return new DomainEventStreamAdapter<TAggregateRoot, TAggregateRootId>(
				new PersistedEventStreamCreator<TAggregateRoot>(
					session,
					bucketId,
					new InMemoryDomainEventTracker<TAggregateRoot>(postPersistenceDomainEventTracker),
					aggregateRoot => idGetter.GetIdFrom(aggregateRoot).ToString(), domainEvent => new EventMessage { Body = domainEvent }),
				new PersistedEventStreamOpener<TAggregateRoot, TAggregateRootId>(
					session,
					bucketId,
					aggregateRootId => aggregateRootId.ToString(),
					postPersistenceDomainEventTracker,
					domainEvent => new EventMessage { Body = domainEvent },
					new PersistedEventToDomainEventReplayAdapter<TAggregateRoot>(
						storedDomainEvent => storedDomainEvent.Body,
						new DummyDomainEventReplay<TAggregateRoot>() // TODO: CREATE A CHAIN OF RESPONSIBILITY THE IN Ichnaea ASSEMBLY, TAKING AN IEnumerable<>
					)));
		}

		private class DummyDomainEventReplay<TAggregateRoot>: IReplayDomainEvents<TAggregateRoot> where TAggregateRoot: class
		{
			public bool CanReplay(TAggregateRoot aggregateRoot, object domainEvent)
			{
				return false;
			}

			public TAggregateRoot Replay(TAggregateRoot aggregateRoot, object domainEvent)
			{
				return null;
			}
		}
	}
}
