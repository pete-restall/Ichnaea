using System;

namespace Restall.Ichnaea.Tests
{
	public static class StringGenerator
	{
		public static string AnyNonNull()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
