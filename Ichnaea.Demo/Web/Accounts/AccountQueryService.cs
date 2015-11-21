using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Raven.Client;
using Restall.Ichnaea.Demo.Accounts;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ReflectedByNancyServiceRouting)]
	public class AccountQueryService
	{
		private readonly RouteLinks links;
		private readonly IDocumentSession documents;
		private readonly AccountRepository repository;

		public AccountQueryService(RouteLinks links, IDocumentSession documents, AccountRepository repository)
		{
			this.links = links;
			this.documents = documents;
			this.repository = repository;
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = CodeAnalysisJustification.ReflectedByNancyServiceRouting)]
		public GetAllAccountsResponse GetAllAccounts(GetAllAccountsRequest request)
		{
			return new GetAllAccountsResponse
				{
					Accounts = this.documents
						.Query<AccountSummary, AccountSummaryIndex>()
						.AsProjection<AccountSummary>()
						.ToArray(),

					OpenAccountUri = this.links.Relative("OpenAccount")
				};
		}

		public GetAccountResponse GetAccount(GetAccountRequest request)
		{
			var surrogateId = this.documents.Load<AccountIdSurrogate>(request.Id);
			var account = this.repository.GetBySortCodeAndAccountNumber(surrogateId.SortCode, surrogateId.AccountNumber);
			return new GetAccountResponse
				{
					Id = request.Id,
					SortCode = account.Id.SortCode,
					AccountNumber = account.Id.AccountNumber,
					Holder = account.Holder,
					Balance = account.Balance
				};
		}
	}
}
