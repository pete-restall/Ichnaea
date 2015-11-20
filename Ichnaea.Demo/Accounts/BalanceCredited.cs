using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Accounts
{
	public class BalanceCredited: AccountTransactionEvent
	{
		public BalanceCredited(decimal amount, string description):
			base(amount, description)
		{
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.JsonSerialisation)]
		private BalanceCredited() { }
	}
}
