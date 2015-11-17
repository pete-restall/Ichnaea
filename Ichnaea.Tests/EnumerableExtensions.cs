using System.Collections.Generic;
using System.Linq;

namespace Restall.Ichnaea.Tests
{
	public static class EnumerableExtensions
	{
		public static T AnyItem<T>(this IEnumerable<T> source)
		{
			return source.Shuffle().First();
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			return source.OrderBy(x => IntegerGenerator.Any());
		}
	}
}
