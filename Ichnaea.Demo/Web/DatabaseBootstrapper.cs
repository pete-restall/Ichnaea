using System.Reflection;
using NEventStore;
using NEventStore.Persistence;
using NEventStore.Persistence.RavenDB;
using NEventStore.Serialization;
using Nancy.TinyIoc;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Database.Server;
using Restall.Ichnaea.NEventStore;

namespace Restall.Ichnaea.Demo.Web
{
	public static class DatabaseBootstrapper
	{
		private const int RavenDatabaseHttpPort = 1234;

		public static void RegisterApplicationScopeDatabaseDependenciesInto(TinyIoCContainer container)
		{
			var documentStore = CreateDocumentStore();
			container.Register(documentStore);
			container.Register(CreateEventStore(documentStore));
		}

		private static IDocumentStore CreateDocumentStore()
		{
			var store = new EmbeddableDocumentStore { ConnectionStringName = "RavenEmbedded", UseEmbeddedHttpServer = true };
			store.Configuration.Port = RavenDatabaseHttpPort;

			NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(RavenDatabaseHttpPort);
			store.Initialize();
			IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), store);

			return store;
		}

		private static IStoreEvents CreateEventStore(IDocumentStore documentStore)
		{
			var persistence = new RavenPersistenceEngine(
				documentStore,
				new DocumentObjectSerializer(),
				new RavenPersistenceOptions());

			return Wireup.Init()
				.With<IPersistStreams>(persistence)
				.Build();
		}

		public static void RegisterRequestScopeDatabaseDependenciesInto(TinyIoCContainer container)
		{
			container.Register(container.Resolve<IDocumentStore>().OpenSession());
			container.Register(new NEventStoreSession(container.Resolve<IStoreEvents>()));
		}
	}
}
