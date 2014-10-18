using System;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class AccountSummary
	{
		public Guid Id { get; set; }
		public string SortCode { get; set; }
		public string AccountNumber { get; set; }
		public string Holder { get; set; }
		public Uri GetDetailsUri { get; set; }
	}
}
