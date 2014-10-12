using System;

namespace Restall.Ichnaea.Fody.Tests
{
	public class AggregateFactory
	{
		private const string AssemblyToProcessNamespace = "Restall.Ichnaea.Fody.AssemblyToProcess";

		public object CreateAggregateWithSingleEvent()
		{
			return CreateAggregateFromClassNamed("AggregateWithSingleEvent");
		}

		private static object CreateAggregateFromClassNamed(string aggregateClassName)
		{
			var type = ModuleWeaverFixture.Assembly.GetType(AssemblyToProcessNamespace + "." + aggregateClassName);
			return Activator.CreateInstance(type);
		}
	}
}
