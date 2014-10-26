using System;

namespace Restall.Ichnaea.Tests
{
	public static class IntegerGenerator
	{
		[ThreadStatic]
		private static Random generator;

		public static int WithinExclusiveRange(int min, int halfOpenMax)
		{
			return Generator.Next(min, halfOpenMax);
		}

		private static Random Generator
		{
			get { return generator ?? (generator = new Random()); }
		}
	}
}
