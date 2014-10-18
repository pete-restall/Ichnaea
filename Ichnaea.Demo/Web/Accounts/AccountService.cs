using System;
using System.Linq;
using Raven.Client;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class AccountService
	{
		private readonly RouteLinks links;

		private readonly IDocumentSession documents;

		public AccountService(RouteLinks links, IDocumentSession documents)
		{
			this.links = links;
			this.documents = documents;
		}

		public GetAllAccountsResponse GetAllAccounts(GetAllAccountsRequest request)
		{
			return new GetAllAccountsResponse
				{
					Accounts = this.documents.Query<AccountSummary>().ToArray(),
					AddAccountUri = this.links.Relative("AddAccount")
				};
		}

		public GetAccountResponse GetAccount(GetAccountRequest request)
		{
			return new GetAccountResponse { Id = request.Id };
		}

		public object AddAccount(AddAccountRequest request)
		{
			if (request.SortCode == null)
				return new IncompleteAddAccountResponse { Request = request };

			return new AddAccountResponse { Id = Guid.NewGuid() };
		}
	}
}
