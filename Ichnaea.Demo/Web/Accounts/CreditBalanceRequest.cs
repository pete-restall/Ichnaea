﻿using System;
using System.Diagnostics.CodeAnalysis;
using NullGuard;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[NamedRoute(RouteNames.CreditBalance, "accounts/{id:guid}/credit", "POST")]
	[NullGuard(ValidationFlags.None)]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class CreditBalanceRequest
	{
		public Guid Id { get; set; }

		public decimal Amount { get; set; }

		public string Description { get; set; }
	}
}
