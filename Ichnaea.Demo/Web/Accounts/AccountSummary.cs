using System;
using NullGuard;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[NullGuard(ValidationFlags.None)]
	public class AccountSummary
	{
		public Guid Id { get; set; }

		public string SortCode { get; set; }

		public string AccountNumber { get; set; }

		public string Holder { get; set; }

		public Uri GetDetailsUri { get; set; }
	}
}
