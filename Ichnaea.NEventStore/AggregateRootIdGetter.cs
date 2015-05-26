namespace Restall.Ichnaea.NEventStore
{
	public delegate string AggregateRootIdGetter<in TAggregateRoot>(TAggregateRoot aggregateRoot); // TODO: Make return value an object ?
}
