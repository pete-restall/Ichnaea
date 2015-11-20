using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class AggregateRootWithInternalId
	{
		public const string PropertyName = nameof(Id);

		public AggregateRootWithInternalId(string id)
		{
			this.Id = id;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		internal string Id { get; }
	}
}
