using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Restall.Ichnaea.Tests
{
	public static class Info
	{
		public static PropertyInfo OfProperty<T>(Expression<Func<T, object>> accessor)
		{
			return PropertyInfo(accessor.Body);
		}

		private static PropertyInfo PropertyInfo(Expression accessor)
		{
			return (PropertyInfo) ((MemberExpression) accessor).Member;
		}

		public static PropertyInfo OfProperty(Expression<Func<object>> accessor)
		{
			return PropertyInfo(accessor.Body);
		}
	}
}
