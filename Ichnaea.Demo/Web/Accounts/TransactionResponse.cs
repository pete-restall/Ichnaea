using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class TransactionResponse
	{
		public Guid AccountId { get; set; }

		public Uri GetAccountUri { get; set; }
	}
}
