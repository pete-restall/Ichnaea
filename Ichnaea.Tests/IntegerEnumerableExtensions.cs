using System;
using System.Collections.Generic;

namespace Restall.Ichnaea.Tests
{
	public static class IntegerEnumerableExtensions
	{
		public static IEnumerable<T> Select<T>(this int times, Func<T> selector)
		{
			for (int i = 0; i < times; i++)
				yield return selector();
		}

		public static void Repeat(this int times, Action<int> action)
		{
			for (int i = 0; i < times; i++)
				action(i);
		}
	}
}
