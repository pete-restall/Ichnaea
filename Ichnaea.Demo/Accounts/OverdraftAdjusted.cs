using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Accounts
{
	public class OverdraftAdjusted: AccountEvent
	{
		public OverdraftAdjusted(decimal amount)
		{
			this.Amount = amount;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.JsonSerialisation)]
		private OverdraftAdjusted() { }

		public decimal Amount { get; private set; }
	}
}
