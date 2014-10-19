using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;

namespace Restall.Ichnaea.Fody.Tests
{
	public static class ModuleWeaverFixture
	{
#if DEBUG
		private const string BuildConfiguration = "Debug";
#else
		private const string BuildConfiguration = "Release";
#endif

		static ModuleWeaverFixture()
		{
			SetAssemblyFilenames();
			File.Copy(OriginalAssemblyFilename, WovenAssemblyFilename, true);
			WeaveAssembly();
			LoadWovenAssembly();
			AggregateRootFactory = new AggregateRootFactory();
		}

		private static void SetAssemblyFilenames()
		{
			var projectAbsoluteDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\AssemblyToProcess"));
			OriginalAssemblyFilename = Path.Combine(projectAbsoluteDirectory, "bin", BuildConfiguration, "AssemblyToProcess.dll");
			WovenAssemblyFilename = OriginalAssemblyFilename.Replace(".dll", ".Woven.dll");
		}

		private static void WeaveAssembly()
		{
			var moduleDefinition = ModuleDefinition.ReadModule(WovenAssemblyFilename);
			var weavingTask = new ModuleWeaver
			{
				ModuleDefinition = moduleDefinition
			};

			weavingTask.Execute();
			moduleDefinition.Write(WovenAssemblyFilename);
		}

		private static void LoadWovenAssembly()
		{
			Assembly = Assembly.LoadFile(WovenAssemblyFilename);
		}

		public static AggregateRootFactory AggregateRootFactory { get; private set; }

		public static string OriginalAssemblyFilename { get; private set; }

		public static string WovenAssemblyFilename { get; private set; }

		public static Assembly Assembly { get; private set; }
	}
}
