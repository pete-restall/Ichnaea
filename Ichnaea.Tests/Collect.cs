using System;

namespace Restall.Ichnaea.Tests
{
	public static class Collect
	{
		public static void Garbage()
		{
			GC.WaitForFullGCComplete();
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
			GC.WaitForFullGCComplete();
			GC.WaitForPendingFinalizers();
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
		}
	}
}
