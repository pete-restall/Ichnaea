using System;
using FluentAssertions;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public static class FodyNotWovenSourceOfExceptionAssertionExtensions
	{
		public static void ShouldThrowFodyNotWovenSourceOfException(this Action action, string because = "", params object[] reasonArgs)
		{
			action
				.ShouldThrow<NotImplementedException>(because, reasonArgs)
				.Where(ex => ex.Message.Contains("Source.Event") && ex.Message.Contains("Fody"), because, reasonArgs);
		}
	}
}
