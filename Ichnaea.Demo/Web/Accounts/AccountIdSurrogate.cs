using System;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class AccountIdSurrogate
	{
		public AccountIdSurrogate(Guid id, string sortCode, string accountNumber)
		{
			this.Id = id;
			this.SortCode = sortCode;
			this.AccountNumber = accountNumber;
		}

		public Guid Id { get; }

		public string SortCode { get; }

		public string AccountNumber { get; }
	}
}
