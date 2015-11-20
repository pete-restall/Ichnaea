using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Accounts
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.JsonSerialisation)]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.JsonSerialisation)]
	public class BalanceDebitRejected: AccountTransactionEvent
	{
		public BalanceDebitRejected(decimal amount, string description, string reason):
			base(amount, description)
		{
			this.Reason = reason;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.JsonSerialisation)]
		private BalanceDebitRejected() { }

		public string Reason { get; private set; }
	}
}
