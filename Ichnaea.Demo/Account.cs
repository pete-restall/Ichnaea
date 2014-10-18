using System;
using System.Text.RegularExpressions;

namespace Restall.Ichnaea.Demo
{
	[Aggregate]
	public class Account
	{
		public Account(string sortCode, string accountNumber, string holder)
		{
			if (!Regex.IsMatch(sortCode, @"^\d\d-\d\d-\d\d$"))
				throw new ArgumentException("Sort Codes are three groups of two digits separated by hyphens, ie. 01-23-45", "sortCode");

			if (!Regex.IsMatch(accountNumber, @"^\d{8}$"))
				throw new ArgumentException("Account Numbers are eight digits, ie. 01234567", "accountNumber");

			if (holder.Trim().Length == 0)
				throw new ArgumentException("Account Holder names cannot be empty", "holder");

			this.Id = Guid.NewGuid();
			this.SortCode = sortCode;
			this.AccountNumber = accountNumber;
			this.Holder = holder;
		}

		public void Credit(decimal amount, string description)
		{
			EnsureAmountIsPositiveOrZero(amount);

			this.Balance += amount;
			Source.Event.Of(new AccountCredited(amount, description));
		}

		private static void EnsureAmountIsPositiveOrZero(decimal amount)
		{
			if (amount < 0)
				throw new ArgumentOutOfRangeException("amount", amount, "Monetary amounts must be positive");
		}

		public void Debit(decimal amount, string description)
		{
			EnsureAmountIsPositiveOrZero(amount);

			if (this.DebitWouldExceedOverdraft(amount))
			{
				Source.Event.Of(new AccountDebitRejected(amount, description, "Overdraft would be exceeded"));
				return;
			}

			this.Balance -= amount;
			Source.Event.Of(new AccountDebited(amount, description));
		}

		private bool DebitWouldExceedOverdraft(decimal amount)
		{
			return this.Balance - amount < -this.Overdraft;
		}

		public void AdjustOverdraft(decimal amount)
		{
			if (this.Overdraft + amount < 0)
				throw new ArgumentOutOfRangeException("amount", amount, "Adjustment would cause a negative overdraft");

			this.Overdraft += amount;
			Source.Event.Of(new OverdraftAdjusted(amount));
		}

		public Guid Id { get; private set; }

		public string SortCode { get; private set; }

		public string AccountNumber { get; private set; }

		public string Holder { get; private set; }

		public decimal Balance { get; private set; }

		public decimal Overdraft { get; private set; }

		public event Source.Of<AccountEvent> EventSource;
	}
}
