using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class AggregateRootWithProtectedInternalId
	{
		public const string PropertyName = nameof(Id);

		public AggregateRootWithProtectedInternalId(string id)
		{
			this.Id = id;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		protected internal string Id { get; }
	}
}
