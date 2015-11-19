using System.Collections.Generic;
using NSubstitute;

namespace Restall.Ichnaea.Tests
{
	public static class EnumerableTestDoubles
	{
		public static IEnumerable<T> Mock<T>()
		{
			return Mock<T>(new T[0]);
		}

		public static IEnumerable<T> Mock<T>(params T[] enumerable)
		{
			var mock = Substitute.For<IEnumerable<T>>();
			mock.GetEnumerator().Returns(_ => ((IEnumerable<T>) enumerable).GetEnumerator());
			return mock;
		}
	}
}
