using System;
using System.Diagnostics.CodeAnalysis;
using NullGuard;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[NamedRoute(RouteNames.DebitBalance, "accounts/{id:guid}/debit", "POST")]
	[NullGuard(ValidationFlags.None)]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class DebitBalanceRequest
	{
		public Guid Id { get; set; }

		public decimal Amount { get; set; }

		public string Description { get; set; }
	}
}
