namespace Restall.Ichnaea.Demo.Accounts.Replay
{
	public class AccountOpenedReplay: TypedDomainEventReplay<Account, AccountOpened>
	{
		public AccountOpenedReplay(): base(false)
		{
		}

		protected override Account Replay(Account aggregateRoot, AccountOpened domainEvent)
		{
			return new Account(new AccountId(domainEvent.SortCode, domainEvent.AccountNumber), domainEvent.Holder);
		}
	}
}
