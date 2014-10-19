using System;

namespace Restall.Ichnaea.Fody.Tests
{
	public class AggregateRootFactory
	{
		private const string AssemblyToProcessNamespace = "Restall.Ichnaea.Fody.AssemblyToProcess";

		public object CreateAggregateRootWithSingleEvent()
		{
			return CreateAggregateRootFromClassNamed("AggregateRootWithSingleEvent");
		}

		private static object CreateAggregateRootFromClassNamed(string aggregateRootClassName)
		{
			var type = ModuleWeaverFixture.Assembly.GetType(AssemblyToProcessNamespace + "." + aggregateRootClassName);
			return Activator.CreateInstance(type);
		}
	}
}
