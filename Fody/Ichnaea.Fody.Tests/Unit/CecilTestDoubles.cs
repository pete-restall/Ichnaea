using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mono.Cecil;

namespace Restall.Ichnaea.Fody.Tests.Unit
{
	public static class CecilTestDoubles
	{
		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
		private class ClassWithEvent
		{
			[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public void Method()
			{
			}

#pragma warning disable 67
			[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public event Source.Of<object> EventSource;
#pragma warning restore 67
		}

		private static readonly ModuleDefinition Module = ModuleDefinition.ReadModule(Assembly.GetExecutingAssembly().Location);
		private static readonly TypeDefinition Type = Module.GetType(typeof(CecilTestDoubles).FullName).NestedTypes[0];

		public static MethodDefinition DummyMethodDefinition()
		{
			return Type.Methods[0];
		}

		public static EventDefinition DummyEventDefinition()
		{
			return Type.Events[0];
		}
	}
}
