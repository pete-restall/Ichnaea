using System;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	public class SomethingHappened
	{
		public SomethingHappened(Guid token)
		{
			this.Token = token;
		}

		public Guid Token { get; private set; }
	}
}
