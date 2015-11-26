using Nancy.TinyIoc;
using NEventStore;
using Restall.Ichnaea.Demo.Accounts;
using Restall.Ichnaea.Demo.Accounts.Replay;
using Restall.Ichnaea.NEventStore;

namespace Restall.Ichnaea.Demo.Web
{
	public static class IchnaeaBootstrapper
	{
		public static void RegisterRequestScopeIchnaeaDependenciesInto(TinyIoCContainer container)
		{
			// TODO: PROPER FLUENT / CONVENTION-BASED CONFIGURATION
			// TODO: ADD AN IgnoredDomainEventReplay<TDomainEventType>, FOR HANDLING SUCH CASES AS OverdraftAdjustmentRejected
			container.RegisterAggregateRoot(
				new NamedPropertyAggregateRootIdGetter<Account, AccountId>("Id"),
				new DomainEventReplayChain<Account>(
					new AccountOpenedReplay(),
					new BalanceCreditedReplay(),
					new BalanceDebitedReplay(),
					new OverdraftAdjustedReplay()));
		}

		private static void RegisterAggregateRoot<TAggregateRoot, TAggregateRootId>(
			this TinyIoCContainer container, IAggregateRootIdGetter<TAggregateRoot, TAggregateRootId> idGetter, IReplayDomainEvents<TAggregateRoot> domainEventReplay)
			where TAggregateRoot: class
		{
			var postPersistenceDomainEventTracker = new LambdaPostPersistenceDomainEventTracker<TAggregateRoot>();
			var prePersistenceDomainEventTracker = new InMemoryDomainEventTracker<TAggregateRoot>(postPersistenceDomainEventTracker);
			container.Register((ctx, _) => CreateDomainEventStream(
				ctx.Resolve<NEventStoreSession>(),
				idGetter,
				prePersistenceDomainEventTracker,
				postPersistenceDomainEventTracker,
				domainEventReplay));

			container.Register<IDomainEventTracker<TAggregateRoot>>(prePersistenceDomainEventTracker);
		}

		private static IDomainEventStream<TAggregateRoot, TAggregateRootId> CreateDomainEventStream<TAggregateRoot, TAggregateRootId>(
			IStoreEvents session,
			IAggregateRootIdGetter<TAggregateRoot, TAggregateRootId> idGetter,
			IPrePersistenceDomainEventTracker<TAggregateRoot> prePersistenceDomainEventTracker,
			IPostPersistenceDomainEventTracker<TAggregateRoot> postPersistenceDomainEventTracker,
			IReplayDomainEvents<TAggregateRoot> domainEventReplay)
			where TAggregateRoot: class
		{
			string bucketId = typeof(TAggregateRoot).Name.Pluralise();
			return new DomainEventStreamAdapter<TAggregateRoot, TAggregateRootId>(
				new PersistedEventStreamCreator<TAggregateRoot>(
					session,
					bucketId,
					prePersistenceDomainEventTracker,
					aggregateRoot => idGetter.GetIdFrom(aggregateRoot).ToString(), domainEvent => new EventMessage { Body = domainEvent }),
				new PersistedEventStreamOpener<TAggregateRoot, TAggregateRootId>(
					session,
					bucketId,
					aggregateRootId => aggregateRootId.ToString(),
					postPersistenceDomainEventTracker,
					domainEvent => new EventMessage { Body = domainEvent },
					new PersistedEventToDomainEventReplayAdapter<TAggregateRoot>(
						storedDomainEvent => storedDomainEvent.Body,
						domainEventReplay
					)));
		}
	}
}
