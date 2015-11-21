using System.Diagnostics.CodeAnalysis;
using NullGuard;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[NamedRoute(RouteNames.OpenAccount, "accounts/open", "GET")]
	[Route("accounts/open", "POST")]
	[NullGuard(ValidationFlags.None)]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class OpenAccountRequest
	{
		public string SortCode { get; set; }

		public string AccountNumber { get; set; }

		public string Holder { get; set; }
	}
}
