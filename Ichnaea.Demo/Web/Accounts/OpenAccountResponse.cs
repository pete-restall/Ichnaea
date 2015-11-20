using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Web.Accounts
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.DtoParticipatesInSerialisation)]
	public class OpenAccountResponse
	{
		public Guid Id { get; set; }
	}
}
