using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Accounts
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.JsonSerialisation)]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.JsonSerialisation)]
	public abstract class AccountEvent
	{
		protected AccountEvent()
		{
			this.Timestamp = DateTime.Now;
		}

		public DateTime Timestamp { get; private set; }
	}
}
