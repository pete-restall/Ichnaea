using System.Reflection;
using NEventStore;
using NEventStore.Persistence;
using NEventStore.Persistence.RavenDB;
using NEventStore.Serialization;
using Nancy.TinyIoc;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Restall.Ichnaea.NEventStore;

namespace Restall.Ichnaea.Demo.Web
{
	public static class DatabaseBootstrapper
	{
		private const string RavenDatabaseDirectory = @"~\App_Data\Database";

		public static void RegisterApplicationScopeDatabaseDependenciesInto(TinyIoCContainer container)
		{
			var documentStore = CreateDocumentStore();
			container.Register(documentStore);
			container.Register(CreateEventStore(documentStore));
		}

		private static DocumentStore CreateDocumentStore()
		{
			var store = new EmbeddableDocumentStore { DataDirectory = RavenDatabaseDirectory };
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
				.With<ICommitEvents>(persistence)
				.With<IAccessSnapshots>(persistence)
				.Build();
		}

		public static void RegisterRequestScopeDatabaseDependenciesInto(TinyIoCContainer container)
		{
			container.Register(container.Resolve<DocumentStore>().OpenSession());
			container.Register(new NEventStoreSession(container.Resolve<IStoreEvents>()));
		}
	}
}
