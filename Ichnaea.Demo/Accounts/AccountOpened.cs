using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Accounts
{
	public class AccountOpened: AccountEvent
	{
		public AccountOpened(AccountId id, string holder)
		{
			this.SortCode = id.SortCode;
			this.AccountNumber = id.AccountNumber;
			this.Holder = holder;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.JsonSerialisation)]
		private AccountOpened() { }

		public string SortCode { get; private set; }

		public string AccountNumber { get; private set; }

		public string Holder { get; private set; }
	}
}
