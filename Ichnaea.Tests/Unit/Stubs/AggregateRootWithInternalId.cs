namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class AggregateRootWithInternalId
	{
		public const string PropertyName = nameof(Id);

		public AggregateRootWithInternalId(string id)
		{
			this.Id = id;
		}

		internal string Id { get; }
	}
}
