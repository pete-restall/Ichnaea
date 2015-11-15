namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class AggregateRootWithProtectedId
	{
		public const string PropertyName = nameof(Id);

		public AggregateRootWithProtectedId(string id)
		{
			this.Id = id;
		}

		protected string Id { get; }
	}
}
