using System;

namespace Restall.Ichnaea.Tests
{
	public static class TypeGenerator
	{
		private static readonly Type[] SerialisableTypes = new[] {typeof(int), typeof(string), typeof(Guid), typeof(object)};

		public static Type AnyReflectable()
		{
			return SerialisableTypes.AnyItem();
		}
	}
}
