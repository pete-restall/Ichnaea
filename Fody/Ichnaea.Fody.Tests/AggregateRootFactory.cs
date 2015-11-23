using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Fody.Tests
{
	public class AggregateRootFactory
	{
		private const string AssemblyToProcessNamespace = "Restall.Ichnaea.Fody.AssemblyToProcess";

		[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global", Justification = CodeAnalysisJustification.FacilitatesInjection)]
		public object CreateAggregateRootFromClassNamed(string aggregateRootClassName)
		{
			var type = ModuleWeaverFixture.Assembly.GetType(AssemblyToProcessNamespace + "." + aggregateRootClassName);
			return Activator.CreateInstance(type);
		}
	}
}
