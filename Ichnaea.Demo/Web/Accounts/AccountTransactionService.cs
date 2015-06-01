using Raven.Client;
using Restall.Ichnaea.Demo.Accounts;
using Restall.Ichnaea.NEventStore;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class AccountTransactionService
	{
		private readonly IDocumentSession documents;
		private readonly NEventStoreSession eventStore;
		private readonly AccountRepository repository;

		public AccountTransactionService(IDocumentSession documents, NEventStoreSession eventStore, AccountRepository repository)
		{
			this.documents = documents;
			this.eventStore = eventStore;
			this.repository = repository;
		}

		public TransactionResponse Credit(CreditBalanceRequest request)
		{
			var surrogateId = this.documents.Load<AccountIdSurrogate>(request.Id);
			var account = this.repository.GetBySortCodeAndAccountNumber(surrogateId.SortCode, surrogateId.AccountNumber);
			account.Credit(request.Amount, request.Description);
			this.eventStore.Commit();

			return new TransactionResponse { };
		}
	}
}
