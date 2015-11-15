namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class AggregateRootWithProtectedInternalId
	{
		public const string PropertyName = nameof(Id);

		public AggregateRootWithProtectedInternalId(string id)
		{
			this.Id = id;
		}

		protected internal string Id { get; }
	}
}
