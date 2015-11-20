using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Accounts
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.JsonSerialisation)]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.JsonSerialisation)]
	public class OverdraftAdjustmentRejected: AccountEvent
	{
		public OverdraftAdjustmentRejected(decimal amount, string reason)
		{
			this.Amount = amount;
			this.Reason = reason;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.JsonSerialisation)]
		private OverdraftAdjustmentRejected() { }

		public decimal Amount { get; private set; }

		public string Reason { get; private set; }
	}
}
