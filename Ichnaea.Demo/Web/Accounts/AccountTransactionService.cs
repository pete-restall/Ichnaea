using System;
using System.Diagnostics.CodeAnalysis;
using Raven.Client;
using Restall.Ichnaea.Demo.Accounts;
using Restall.Ichnaea.NEventStore;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ReflectedByNancyServiceRouting)]
	public class AccountTransactionService
	{
		private readonly RouteLinks links;
		private readonly IDocumentSession documents;
		private readonly NEventStoreSession eventStore;
		private readonly AccountRepository repository;

		public AccountTransactionService(RouteLinks links, IDocumentSession documents, NEventStoreSession eventStore, AccountRepository repository)
		{
			this.links = links;
			this.documents = documents;
			this.eventStore = eventStore;
			this.repository = repository;
		}

		public TransactionResponse Credit(CreditBalanceRequest request)
		{
			var account = this.GetAccountById(request.Id);
			account.Credit(request.Amount, request.Description);
			this.eventStore.Commit();

			return this.TransactionResponse(request.Id);
		}

		private Account GetAccountById(Guid id)
		{
			var surrogateId = this.documents.Load<AccountIdSurrogate>(id);
			return this.repository.GetBySortCodeAndAccountNumber(surrogateId.SortCode, surrogateId.AccountNumber);
		}

		private TransactionResponse TransactionResponse(Guid id)
		{
			return new TransactionResponse
				{
					AccountId = id,
					GetAccountUri = this.links.Relative(RouteNames.GetAccount, new { id })
				};
		}

		public TransactionResponse Debit(DebitBalanceRequest request)
		{
			var account = this.GetAccountById(request.Id);
			account.Debit(request.Amount, request.Description);
			this.eventStore.Commit();

			return this.TransactionResponse(request.Id);
		}
	}
}
