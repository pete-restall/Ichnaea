using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Accounts
{
	public class OverdraftAdjustmentRejected: AccountEvent
	{
		public OverdraftAdjustmentRejected(decimal amount, string reason)
		{
			this.Amount = amount;
			this.Reason = reason;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "JSON Serialisation")]
		private OverdraftAdjustmentRejected() { }

		public decimal Amount { get; private set; }

		public string Reason { get; private set; }
	}
}
