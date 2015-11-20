using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class IncompleteOpenAccountResponse
	{
		public OpenAccountRequest Request { get; set; }

		public Uri OpenAccountUri { get; set; }

		public string Message { get; set; }
	}
}
