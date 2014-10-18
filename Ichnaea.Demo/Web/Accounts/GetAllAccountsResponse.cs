using System;
using System.Collections.Generic;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class GetAllAccountsResponse
	{
		public IEnumerable<AccountSummary> Accounts { get; set; }
		public Uri AddAccountUri { get; set; }
	}
}
