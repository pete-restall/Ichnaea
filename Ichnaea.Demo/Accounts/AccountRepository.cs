﻿namespace Restall.Ichnaea.Demo.Accounts
{
	public class AccountRepository
	{
		private readonly IDomainEventStream<Account> stream;

		public AccountRepository(IDomainEventStream<Account> stream)
		{
			this.stream = stream;
		}

		public void Add(Account account)
		{
			this.stream.CreateFrom(account);
		}

		public Account GetBySortCodeAndAccountNumber(string sortCode, string accountNumber)
		{
			return this.stream.Replay(new AccountId(sortCode, accountNumber).ToString());
		}
	}
}
