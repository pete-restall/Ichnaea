using System;

namespace Restall.Ichnaea.Demo.Accounts
{
	public abstract class AccountEvent
	{
		protected AccountEvent()
		{
			this.Timestamp = DateTime.Now;
		}

		public DateTime Timestamp { get; }
	}
}
