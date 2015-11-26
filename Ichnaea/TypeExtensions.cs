using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restall.Ichnaea
{
	internal static class TypeExtensions
	{
		public static IEnumerable<FieldInfo> GetAllFields(this Type type, BindingFlags bindingFlags)
		{
			return type?.GetFields(bindingFlags).Concat(GetAllFields(type.BaseType, bindingFlags)) ?? new FieldInfo[0];
		}

		public static IEnumerable<PropertyInfo> GetAllProperties(this Type type, BindingFlags bindingFlags)
		{
			return type?.GetProperties(bindingFlags).Concat(GetAllProperties(type.BaseType, bindingFlags)) ?? new PropertyInfo[0];
		}
	}
}
