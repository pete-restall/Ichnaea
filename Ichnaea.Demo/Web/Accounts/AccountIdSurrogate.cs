using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class AccountIdSurrogate
	{
		public AccountIdSurrogate(Guid surrogateId, string sortCode, string accountNumber)
		{
			this.SurrogateId = surrogateId;
			this.SortCode = sortCode;
			this.AccountNumber = accountNumber;
		}

		public Guid SurrogateId { get; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.UsedInView)]
		public string SortCode { get; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.UsedInView)]
		public string AccountNumber { get; }
	}
}
