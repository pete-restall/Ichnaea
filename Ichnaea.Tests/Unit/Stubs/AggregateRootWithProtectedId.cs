using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class AggregateRootWithProtectedId
	{
		public const string PropertyName = nameof(Id);

		public AggregateRootWithProtectedId(string id)
		{
			this.Id = id;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		protected string Id { get; }
	}
}
