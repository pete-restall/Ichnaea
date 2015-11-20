using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Web.Home
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class IndexResponse
	{
		public Uri GetAllAccountsUri { get; set; }
	}
}
