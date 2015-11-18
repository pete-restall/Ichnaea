using System;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class GetAccountResponse
	{
		public Guid Id { get; set; }

		public string SortCode { get; set; }

		public string AccountNumber { get; set; }

		public string Holder { get; set; }

		public decimal Balance { get; set; }
	}
}
