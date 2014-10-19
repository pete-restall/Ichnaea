namespace Restall.Ichnaea.Demo.Accounts
{
	public class AccountFactory
	{
		private readonly IDomainEventTracker<Account> tracker;

		public AccountFactory(IDomainEventTracker<Account> tracker)
		{
			this.tracker = tracker;
		}

		public Account Create(string sortCode, string accountNumber, string holder)
		{
			var account = new Account(new AccountId(sortCode, accountNumber), holder);
			this.tracker.AggregateRootCreated(account, new AccountOpened(account.Id, holder));
			return account;
		}
	}
}
