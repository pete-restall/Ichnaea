using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class GetAllAccountsResponse
	{
		public IEnumerable<AccountSummary> Accounts { get; set; }

		public Uri OpenAccountUri { get; set; }
	}
}
