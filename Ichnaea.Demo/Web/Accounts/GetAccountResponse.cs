using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class GetAccountResponse
	{
		public Guid Id { get; set; }

		public string SortCode { get; set; }

		public string AccountNumber { get; set; }

		public string Holder { get; set; }

		public decimal Balance { get; set; }

		public decimal Overdraft { get; set; }

		public Uri CreditAccountUri { get; set; }

		public Uri DebitAccountUri { get; set; }

		public Uri GetAllAccountsUri { get; set; }
	}
}
