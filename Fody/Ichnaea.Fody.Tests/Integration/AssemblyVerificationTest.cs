using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public class AssemblyVerificationTest
	{
		private static readonly string[] PeVerifyPossibleLocations =
			{
				@"%programfiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\PEVerify.exe",
				@"%programfiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\PEVerify.exe"
			};

		private const int ReasonablePeVerifyTimeoutMilliseconds = 10000;

		[Fact(Timeout = ReasonablePeVerifyTimeoutMilliseconds)]
		public void ExpectOriginalAssemblyIsValidAndVerifiable()
		{
			VerifyAssembly(ModuleWeaverFixture.OriginalAssemblyFilename);
		}

		private static void VerifyAssembly(string assemblyFilename)
		{
			var process = RunPeVerifyInNewProcess(assemblyFilename);
			process.ExitCode.Should().Be(
				0,
				"because PEVerify.exe should not find errors in assembly {0}:\n\n{1}",
				assemblyFilename,
				process.StandardOutput.ReadToEnd());
		}

		private static Process RunPeVerifyInNewProcess(string assemblyFilename)
		{
			var exeFilename = GetPeVerifyExecutableFilename();
			var process = Process.Start(
				new ProcessStartInfo(exeFilename, "\"" + assemblyFilename + "\"")
					{
						RedirectStandardOutput = true,
						UseShellExecute = false,
						CreateNoWindow = true
					});

			if (process == null)
				throw new InvalidOperationException("Process.Start() returned null !");

			process.WaitForExit(ReasonablePeVerifyTimeoutMilliseconds).Should().BeTrue("because PEVerify.exe shouldn't have taken so long");
			return process;
		}

		private static string GetPeVerifyExecutableFilename()
		{
			var possibleFilenames = PeVerifyPossibleLocations.Select(Environment.ExpandEnvironmentVariables).ToArray();
			var filename = possibleFilenames.FirstOrDefault(File.Exists);
			filename.Should().NotBeNull(
				"because PEVerify.exe should be on the build machine, but was not found in these locations:\n\t",
				string.Join("\n\t", possibleFilenames));

			return filename;
		}

		[Fact(Timeout = ReasonablePeVerifyTimeoutMilliseconds)]
		public void ExpectWovenAssemblyIsValidAndVerifiable()
		{
			VerifyAssembly(ModuleWeaverFixture.WovenAssemblyFilename);
		}
	}
}
