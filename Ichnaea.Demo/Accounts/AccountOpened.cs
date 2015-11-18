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

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "JSON Serialisation")]
		private AccountOpened() { }

		public string SortCode { get; }

		public string AccountNumber { get; }

		public string Holder { get; }
	}
}
