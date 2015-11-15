using System;

namespace Restall.Ichnaea.Tests
{
	public static class Collect
	{
		public static void Garbage()
		{
			GC.WaitForFullGCComplete();
			ForceBlockingCollection();
			ForceBlockingCollection();
		}

		private static void ForceBlockingCollection()
		{
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
			GC.WaitForFullGCComplete();
			GC.WaitForPendingFinalizers();
		}
	}
}
