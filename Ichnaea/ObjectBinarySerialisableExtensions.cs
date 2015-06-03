using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Restall.Ichnaea
{
	internal static class ObjectBinarySerialisableExtensions
	{
		public static bool IsBinarySerialisable(this object obj)
		{
			return obj != null && (
				obj.GetType().GetCustomAttribute<SerializableAttribute>() != null ||
				obj.GetType().GetInterfaces().Any(type => type == typeof(ISerializable)));
		}
	}
}
