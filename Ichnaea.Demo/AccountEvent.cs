using System;

namespace Restall.Ichnaea.Demo
{
	public abstract class AccountEvent
	{
		protected AccountEvent()
		{
			this.Timestamp = DateTime.Now;
		}

		public DateTime Timestamp { get; set; }
	}
}
