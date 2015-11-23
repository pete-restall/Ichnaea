using System;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	public class SomethingDerivedHappened: SomethingHappened
	{
		public SomethingDerivedHappened(Guid token): base(token)
		{
		}
	}
}
