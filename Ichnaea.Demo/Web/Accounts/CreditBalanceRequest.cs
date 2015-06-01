using System;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[Route("accounts/{id:guid}/credit", "POST")]
	public class CreditBalanceRequest
	{
		public Guid Id { get; set; }

		public decimal Amount { get; set; }

		public string Description { get; set; }
	}
}
