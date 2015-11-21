using System;
using FluentAssertions;

namespace Restall.Ichnaea.Tests.Unit
{
	public static class AggregateRootIdNotFoundExceptionAssertionExtensions
	{
		public static void ShouldThrowAggregateRootIdNotFoundExceptionFor<TAggregateRoot, TAggregateRootId>(
			this Action action, string because = "", params object[] reasonArgs)
		{
			action.ShouldThrow<AggregateRootIdNotFoundException>(because, reasonArgs)
				.Where(ex =>
					ex.AggregateRootType == typeof(TAggregateRoot) &&
					ex.AggregateRootIdType == typeof(TAggregateRootId));
		}
	}
}
