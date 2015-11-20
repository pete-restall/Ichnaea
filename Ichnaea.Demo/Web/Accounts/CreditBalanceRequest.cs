using System;
using System.Diagnostics.CodeAnalysis;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[Route("accounts/{id:guid}/credit", "POST")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class CreditBalanceRequest
	{
		public Guid Id { get; set; }

		public decimal Amount { get; set; }

		public string Description { get; set; }
	}
}
