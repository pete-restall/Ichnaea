using System.Linq;
using System.Reflection;

namespace Restall.Ichnaea
{
	public class TypedPropertyAggregateRootIdGetter<TAggregateRoot, TAggregateRootId>: IAggregateRootIdGetter<TAggregateRoot, TAggregateRootId>
		where TAggregateRoot: class
	{
		private readonly PropertyInfo idProperty;

		public TypedPropertyAggregateRootIdGetter()
		{
			var properties = typeof(TAggregateRoot)
				.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(x => x.PropertyType == typeof(TAggregateRootId))
				.ToArray();

			if (properties.Length != 1)
				throw new AggregateRootIdPropertyNotFoundException(typeof(TAggregateRoot), typeof(TAggregateRootId), string.Empty);

			this.idProperty = properties.First();
		}

		public TAggregateRootId GetIdFrom(TAggregateRoot aggregateRoot)
		{
			return (TAggregateRootId) this.idProperty.GetValue(aggregateRoot);
		}
	}
}
