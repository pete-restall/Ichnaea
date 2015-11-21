using System;
using FluentAssertions;

namespace Restall.Ichnaea.Tests.Unit
{
	public static class AggregateRootIdPropertyNotFoundExceptionAssertionExtensions
	{
		public static void ShouldThrowAggregateRootIdPropertyNotFoundExceptionFor<TAggregateRoot, TAggregateRootId>(
			this Action action, string propertyName, string because = "", params object[] reasonArgs)
		{
			action.ShouldThrow<AggregateRootIdPropertyNotFoundException>(because, reasonArgs)
				.Where(ex =>
					ex.PropertyName == propertyName &&
					ex.AggregateRootType == typeof(TAggregateRoot) &&
					ex.AggregateRootIdType == typeof(TAggregateRootId));
		}
	}
}
