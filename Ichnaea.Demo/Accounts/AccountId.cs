using System;
using System.Text.RegularExpressions;

namespace Restall.Ichnaea.Demo.Accounts
{
	public class AccountId
	{
		public AccountId(string sortCode, string accountNumber)
		{
			if (!Regex.IsMatch(sortCode, @"^\d\d-\d\d-\d\d$"))
				throw new ArgumentException("Sort Codes are three groups of two digits separated by hyphens, ie. 01-23-45", nameof(sortCode));

			if (!Regex.IsMatch(accountNumber, @"^\d{8}$"))
				throw new ArgumentException("Account Numbers are eight digits, ie. 01234567", nameof(accountNumber));

			this.SortCode = sortCode;
			this.AccountNumber = accountNumber;
		}

		public string SortCode { get; private set; }

		public string AccountNumber { get; private set; }

		public override string ToString()
		{
			return this.SortCode + " / " + this.AccountNumber;
		}
	}
}
