using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class AggregateRootWithShadowedIdBase
	{
		protected AggregateRootWithShadowedIdBase(string id)
		{
			this.Id = id;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public string Id { get; }
	}
}
