using System;
using System.Diagnostics.CodeAnalysis;
using NullGuard;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[NullGuard(ValidationFlags.None)]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class AccountSummary
	{
		public Guid Id { get; set; }

		public string SortCode { get; set; }

		public string AccountNumber { get; set; }

		public string Holder { get; set; }

		public Uri GetDetailsUri { get; set; }
	}
}
