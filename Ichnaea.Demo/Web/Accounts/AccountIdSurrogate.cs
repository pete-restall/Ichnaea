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

		public Guid SurrogateId { get; private set; }

		public string SortCode { get; private set; }

		public string AccountNumber { get; private set; }
	}
}
