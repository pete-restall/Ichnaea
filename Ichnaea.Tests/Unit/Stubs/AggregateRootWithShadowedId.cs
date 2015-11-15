namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class AggregateRootWithShadowedId : AggregateRootWithShadowedIdBase
	{
		public const string PropertyName = nameof(Id);

		public AggregateRootWithShadowedId(string baseId, string derivedId) : base(baseId)
		{
			this.Id = derivedId;
		}

		public new string Id { get; }
	}
}
