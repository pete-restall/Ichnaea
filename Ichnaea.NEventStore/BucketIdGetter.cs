namespace Restall.Ichnaea.NEventStore
{
	public delegate string BucketIdGetter<in TAggregateRoot>(TAggregateRoot aggregateRoot);
}
