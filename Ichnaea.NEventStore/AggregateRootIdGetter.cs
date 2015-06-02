namespace Restall.Ichnaea.NEventStore
{
	public delegate string AggregateRootIdGetter<in TAggregateRoot>(TAggregateRoot aggregateRoot);
}
