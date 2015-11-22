using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	public class ObjectInitialiserSomethingHappened
	{
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public Guid Token { get; set; }
	}
}
