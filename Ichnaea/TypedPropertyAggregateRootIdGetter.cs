using System.Linq;
using System.Reflection;

namespace Restall.Ichnaea
{
	public class TypedPropertyAggregateRootIdGetter<TAggregateRoot, TAggregateRootId>: IAggregateRootIdGetter<TAggregateRoot, TAggregateRootId>
		where TAggregateRoot: class
	{
		private static readonly PropertyInfo IdProperty;

		static TypedPropertyAggregateRootIdGetter()
		{
			var properties = typeof(TAggregateRoot)
				.GetAllProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(x => x.PropertyType == typeof(TAggregateRootId))
				.ToArray();

			if (properties.Length == 1)
				IdProperty = properties[0];
		}

		public TypedPropertyAggregateRootIdGetter()
		{
			if (IdProperty == null)
				throw new AggregateRootIdNotFoundException(typeof(TAggregateRoot), typeof(TAggregateRootId));
		}

		public TAggregateRootId GetIdFrom(TAggregateRoot aggregateRoot)
		{
			return (TAggregateRootId) IdProperty.GetValue(aggregateRoot);
		}
	}
}
