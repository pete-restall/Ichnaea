using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Accounts
{
	[AggregateRoot]
	public class Account
	{
		public Account(AccountId id, string holder)
		{
			if (holder.Trim().Length == 0)
				throw new ArgumentException("Account Holder names cannot be empty", nameof(holder));

			this.Id = id;
			this.Holder = holder;
		}

		public void Credit(decimal amount, string description)
		{
			EnsureAmountIsPositiveOrZero(amount);

			this.Balance += amount;
			Source.Event.Of(new BalanceCredited(amount, description));
		}

		private static void EnsureAmountIsPositiveOrZero(decimal amount)
		{
			if (amount < 0)
				throw new ArgumentOutOfRangeException(nameof(amount), amount, "Monetary amounts must be positive");
		}

		public void Debit(decimal amount, string description)
		{
			EnsureAmountIsPositiveOrZero(amount);

			decimal newBalance = this.Balance - amount;
			if (newBalance < -this.Overdraft)
			{
				Source.Event.Of(new BalanceDebitRejected(amount, description, "Overdraft would be exceeded"));
				return;
			}

			this.Balance -= amount;
			Source.Event.Of(new BalanceDebited(amount, description));
		}

		public void AdjustOverdraft(decimal amount)
		{
			if (this.Overdraft + amount < 0)
				throw new ArgumentOutOfRangeException(nameof(amount), amount, "Adjustment would cause a negative overdraft");

			decimal newOverdraft = this.Overdraft + amount;
			if (this.Balance < -newOverdraft)
			{
				Source.Event.Of(new OverdraftAdjustmentRejected(amount, "Balance exceeds proposed Overdraft"));
				return;
			}

			this.Overdraft = newOverdraft;
			Source.Event.Of(new OverdraftAdjusted(amount));
		}

		public AccountId Id { get; }

		public string Holder { get; }

		public decimal Balance { get; private set; }

		public decimal Overdraft { get; private set; }

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<AccountEvent> EventSource;
	}
}
