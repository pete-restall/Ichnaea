using Raven.Client;
using Restall.Ichnaea.Demo.Accounts;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class AccountTransactionService
	{
		private readonly IDocumentSession documents;

		private readonly AccountRepository repository;

		public AccountTransactionService(IDocumentSession documents, AccountRepository repository)
		{
			this.documents = documents;
			this.repository = repository;
		}

		public TransactionResponse Credit(CreditBalanceRequest request)
		{
			var surrogateId = this.documents.Load<AccountIdSurrogate>(request.Id);
			var account = this.repository.GetBySortCodeAndAccountNumber(surrogateId.SortCode, surrogateId.AccountNumber);
			account.Credit(request.Amount, request.Description);

			return new TransactionResponse { };
		}
	}
}
