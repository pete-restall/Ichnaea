using System;
using NullGuard;

namespace Restall.Ichnaea.Demo.Accounts.Replay
{
	public class AccountOpenedReplay: IReplayDomainEvents<Account>
	{
		public bool CanReplay([AllowNull] Account aggregateRoot, object domainEvent)
		{
			return domainEvent.GetType() == typeof(AccountOpened);
		}

		public Account Replay([AllowNull] Account aggregateRoot, object domainEvent)
		{
			if (!this.CanReplay(aggregateRoot, domainEvent))
				throw new ArgumentException("Cannot replay Domain Event of type " + domainEvent.GetType(), nameof(domainEvent));

			var accountOpened = (AccountOpened) domainEvent;
			return new Account(new AccountId(accountOpened.SortCode, accountOpened.AccountNumber), accountOpened.Holder);
		}
	}
}
