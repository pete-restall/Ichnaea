using System.Reflection;

namespace Restall.Ichnaea
{
	public class NamedPropertyAggregateRootIdGetter<TAggregateRoot, TAggregateRootId>: IAggregateRootIdGetter<TAggregateRoot, TAggregateRootId>
		where TAggregateRoot: class
	{
		private readonly PropertyInfo property;

		public NamedPropertyAggregateRootIdGetter(string propertyName)
		{
			this.property = typeof(TAggregateRoot).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (this.property == null || this.property.PropertyType != typeof(TAggregateRootId))
				throw new AggregateRootIdPropertyNotFoundException(typeof(TAggregateRoot), typeof(TAggregateRootId), propertyName);
		}

		public TAggregateRootId GetIdFrom(TAggregateRoot aggregateRoot)
		{
			return (TAggregateRootId) this.property.GetValue(aggregateRoot);
		}
	}
}
