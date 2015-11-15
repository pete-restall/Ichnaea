namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class AggregateRootWithShadowedIdBase
	{
		protected AggregateRootWithShadowedIdBase(string id)
		{
			this.Id = id;
		}

		public string Id { get; }
	}
}
