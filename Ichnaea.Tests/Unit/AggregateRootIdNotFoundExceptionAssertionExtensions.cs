using System;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace Restall.Ichnaea.Tests.Unit
{
	public static class AggregateRootIdNotFoundExceptionAssertionExtensions
	{
		public static ExceptionAssertions<AggregateRootIdNotFoundException> ShouldThrowAggregateRootIdNotFoundExceptionFor<TAggregateRoot, TAggregateRootId>(
			this Action action, string because = "", params object[] reasonArgs)
		{
			return action.ShouldThrow<AggregateRootIdNotFoundException>(because, reasonArgs)
				.Where(ex =>
					ex.AggregateRootType == typeof(TAggregateRoot) &&
					ex.AggregateRootIdType == typeof(TAggregateRootId));
		}
	}
}
