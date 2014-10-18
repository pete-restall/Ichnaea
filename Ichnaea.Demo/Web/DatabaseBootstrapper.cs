using NEventStore;
using NEventStore.Persistence;
using NEventStore.Persistence.RavenDB;
using NEventStore.Serialization;
using Nancy.TinyIoc;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace Restall.Ichnaea.Demo.Web
{
	public static class DatabaseBootstrapper
	{
		private const string RavenDatabaseDirectory = @"~\App_Data\Database";

		public static void RegisterDatabaseDependenciesInto(TinyIoCContainer container)
		{
			var documentStore = CreateDocumentStore();
			container.Register(documentStore);
			container.Register((ctx, _) => ctx.Resolve<DocumentStore>().OpenSession());
			container.Register(CreateEventStore(documentStore));
		}

		private static DocumentStore CreateDocumentStore()
		{
			var store = new EmbeddableDocumentStore { DataDirectory = RavenDatabaseDirectory };
			store.Initialize();
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
	}
}
