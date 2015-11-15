namespace Restall.Ichnaea
{
	public interface IAggregateRootIdGetter<in TAggregateRoot, out TAggregateRootId> where TAggregateRoot: class
	{
		TAggregateRootId GetIdFrom(TAggregateRoot aggregateRoot);
	}
}
