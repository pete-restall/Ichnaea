using System;
using Raven.Client;
using Restall.Ichnaea.Demo.Accounts;
using Restall.Ichnaea.NEventStore;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class AccountAdministrationService
	{
		private readonly RouteLinks links;
		private readonly IDocumentSession documents;
		private readonly NEventStoreSession eventStore;
		private readonly AccountFactory factory;
		private readonly AccountRepository repository;

		public AccountAdministrationService(
			RouteLinks links, IDocumentSession documents, NEventStoreSession eventStore, AccountFactory factory, AccountRepository repository)
		{
			this.links = links;
			this.documents = documents;
			this.eventStore = eventStore;
			this.factory = factory;
			this.repository = repository;
		}

		public object OpenAccount(OpenAccountRequest request)
		{
			if (request.SortCode == null || request.AccountNumber == null || request.Holder == null)
			{
				return new IncompleteOpenAccountResponse
					{
						Request = request,
						OpenAccountUri = this.links.Relative("OpenAccount")
					};
			}

			var account = this.factory.Create(request.SortCode, request.AccountNumber, request.Holder);
			this.repository.Add(account);
			this.eventStore.Commit();

			var surrogateId = new AccountIdSurrogate(Guid.NewGuid(), request.SortCode, request.AccountNumber);
			this.documents.Store(surrogateId, surrogateId.SurrogateId.ToString()); // TODO: ToString() WORKS, BUT IT'D BE BETTER TO USE THE GUID AS THE ETAG, TOO...
			this.documents.SaveChanges();

			return new OpenAccountResponse { Id = surrogateId.SurrogateId };
		}
	}
}
