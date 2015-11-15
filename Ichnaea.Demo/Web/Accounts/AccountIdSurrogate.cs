using System;

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

		public string SortCode { get; }

		public string AccountNumber { get; }
	}
}
