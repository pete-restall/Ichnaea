using System;
using System.Collections.Generic;

namespace Restall.Ichnaea.Fody.Tests
{
	public static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
				action(item);
		}
	}
}
