using System;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Fody.Tests.Unit
{
	public class ModuleWeaverTests
	{
		[Fact]
		public void Execute_CalledWhenModuleDefinitionIsNull_ExpectInvalidOperationException()
		{
			new ModuleWeaver().Invoking(x => x.Execute()).ShouldThrow<InvalidOperationException>().Where(ExceptionMessageContainsNullModuleDefinition());
		}

		private static Expression<Func<InvalidOperationException, bool>> ExceptionMessageContainsNullModuleDefinition()
		{
			return x => x.Message.Contains("ModuleDefinition") && x.Message.Contains("null");
		}
	}
}
