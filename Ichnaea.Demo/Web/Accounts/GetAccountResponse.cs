using System;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	public class GetAccountResponse
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Holder { get; set; }

		public decimal Balance { get; set; }
	}
}
