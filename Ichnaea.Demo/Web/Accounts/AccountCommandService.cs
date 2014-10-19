using System;
using Raven.Client;
using Restall.Ichnaea.Demo.Accounts;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class AccountCommandService
	{
		private readonly IDocumentSession documents;

		private readonly AccountFactory factory;

		private readonly AccountRepository repository;

		public AccountCommandService(IDocumentSession documents, AccountFactory factory, AccountRepository repository)
		{
			this.documents = documents;
			this.factory = factory;
			this.repository = repository;
		}

		public object OpenAccount(OpenAccountRequest request)
		{
			if (request.SortCode == null || request.AccountNumber == null || request.Holder == null)
				return new IncompleteOpenAccountResponse { Request = request };

			var account = this.factory.Create(request.SortCode, request.AccountNumber, request.Holder);
			this.repository.Add(account);

			var surrogateId = new AccountIdSurrogate(Guid.NewGuid(), request.SortCode, request.AccountNumber);
			this.documents.Store(surrogateId, surrogateId.SurrogateId);
			this.documents.SaveChanges();

			return new OpenAccountResponse { Id = surrogateId.SurrogateId };
		}
	}
}
