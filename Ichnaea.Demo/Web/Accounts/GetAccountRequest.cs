using System;
using System.Diagnostics.CodeAnalysis;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[NamedRoute(RouteNames.GetAccount, "accounts/{id:guid}")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class GetAccountRequest
	{
		public Guid Id { get; set; }
	}
}
