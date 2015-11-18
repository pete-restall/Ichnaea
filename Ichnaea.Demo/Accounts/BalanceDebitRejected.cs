using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Accounts
{
	public class BalanceDebitRejected: AccountTransactionEvent
	{
		public BalanceDebitRejected(decimal amount, string description, string reason):
			base(amount, description)
		{
			this.Reason = reason;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "JSON Serialisation")]
		private BalanceDebitRejected() { }

		public string Reason { get; }
	}
}
