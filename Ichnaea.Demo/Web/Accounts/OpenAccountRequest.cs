using NullGuard;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[NamedRoute("AddAccount", "/accounts/add", "GET")]
	[Route("/accounts/add", "PUT")]
	[NullGuard(ValidationFlags.None)]
	public class OpenAccountRequest
	{
		public string SortCode { get; set; }
		public string AccountNumber { get; set; }
		public string Holder { get; set; }
	}
}
