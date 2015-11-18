using System;
using NEventStore;
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
				return this.IncompleteOpenAccountResponse(request, "Enter all Account details.");

			var account = this.factory.Create(request.SortCode, request.AccountNumber, request.Holder);
			this.repository.Add(account);

			try
			{
				this.eventStore.Commit();
			}
			catch (ConcurrencyException)
			{
				return this.IncompleteOpenAccountResponse(request, "Account already exists.");
			}

			var surrogateId = new AccountIdSurrogate(Guid.NewGuid(), request.SortCode, request.AccountNumber);
			this.documents.Store(surrogateId, "AccountIdSurrogates/" + surrogateId.SurrogateId);
			this.documents.SaveChanges();

			return new OpenAccountResponse { Id = surrogateId.SurrogateId };
		}

		private object IncompleteOpenAccountResponse(OpenAccountRequest request, string message)
		{
			return new IncompleteOpenAccountResponse
				{
					Request = request,
					OpenAccountUri = this.links.Relative("OpenAccount"),
					Message = message
				};
		}
	}
}
