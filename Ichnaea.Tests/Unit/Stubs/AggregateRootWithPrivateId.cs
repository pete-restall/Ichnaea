namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class AggregateRootWithPrivateId
	{
		public const string PropertyName = nameof(Id);

		public AggregateRootWithPrivateId(string id)
		{
			this.Id = id;
		}

		private string Id { get; }
	}
}
