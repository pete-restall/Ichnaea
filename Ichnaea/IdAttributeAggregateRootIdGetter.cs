using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restall.Ichnaea
{
	public class IdAttributeAggregateRootIdGetter<TAggregateRoot, TAggregateRootId>: IAggregateRootIdGetter<TAggregateRoot, TAggregateRootId>
		where TAggregateRoot: class
	{
		private const BindingFlags MemberBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static IEnumerable<FieldInfo> AllFields => typeof(TAggregateRoot).GetAllFields(MemberBindingFlags);

		private static IEnumerable<PropertyInfo> AllProperties => typeof(TAggregateRoot).GetAllProperties(MemberBindingFlags);

		private static readonly Func<TAggregateRoot, TAggregateRootId> IdGetter;

		static IdAttributeAggregateRootIdGetter()
		{
			var idFields = OnlyDecorated(AllFields).ToArray();
			var idProperties = OnlyDecorated(AllProperties).ToArray();

			if (idFields.Length + idProperties.Length != 1)
				return;

			if (idFields.Length > 0)
				IdGetter = aggregateRoot => (TAggregateRootId) idFields[0].GetValue(aggregateRoot);
			else
				IdGetter = aggregateRoot => (TAggregateRootId) idProperties[0].GetValue(aggregateRoot);
		}

		private static IEnumerable<T> OnlyDecorated<T>(IEnumerable<T> members) where T: MemberInfo
		{
			return members.Where(x => x.GetCustomAttribute<IdAttribute>() != null);
		}

		public IdAttributeAggregateRootIdGetter()
		{
			if (IdGetter == null)
				throw new AggregateRootIdNotFoundException(typeof(TAggregateRoot), typeof(TAggregateRootId));
		}

		public TAggregateRootId GetIdFrom(TAggregateRoot aggregateRoot)
		{
			return IdGetter(aggregateRoot);
		}
	}
}
