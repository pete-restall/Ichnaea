using System;
using System.Runtime.Serialization;

namespace Restall.Ichnaea
{
	internal static class SerializationInfoExtensions
	{
		public static T GetOrDefault<T>(this SerializationInfo info, string name, T defaultValue)
		{
			Type valueType;
			if (!info.TryGet(name + ":type", out valueType))
				return defaultValue;

			return valueType != null && typeof(T).IsAssignableFrom(valueType) ? (T) info.GetValue(name + ":value", valueType) : defaultValue;
		}

		private static bool TryGet<T>(this SerializationInfo info, string name, out T value) where T: class
		{
			foreach (var entry in info)
			{
				if (entry.Name == name)
				{
					value = entry.Value as T;
					return entry.Value != null && value != null;
				}
			}

			value = null;
			return false;
		}

		public static void AddOrDefault(this SerializationInfo info, string name, object value, Func<object, object> defaultValue)
		{
			if (!value.IsBinarySerialisable())
			{
				value = defaultValue(value);
				if (!value.IsBinarySerialisable())
				{
					info.AddValue(name + ":type", typeof(object));
					info.AddValue(name + ":value", null, typeof(object));
					return;
				}
			}

			var valueType = value.GetType();
			info.AddValue(name + ":type", valueType);
			info.AddValue(name + ":value", value, valueType);
		}
	}
}
