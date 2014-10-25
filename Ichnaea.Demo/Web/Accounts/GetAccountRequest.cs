using System;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[NamedRoute("GetAccount", "accounts/{id:guid}")]
	public class GetAccountRequest
	{
		public Guid Id { get; set; }
	}
}
