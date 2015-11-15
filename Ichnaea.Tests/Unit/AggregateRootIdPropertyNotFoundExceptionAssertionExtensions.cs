using System;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace Restall.Ichnaea.Tests.Unit
{
	public static class AggregateRootIdPropertyNotFoundExceptionAssertionExtensions
	{
		public static ExceptionAssertions<AggregateRootIdPropertyNotFoundException> ShouldThrowAggregateRootIdPropertyNotFoundExceptionFor<TAggregateRoot, TAggregateRootId>(
			this Action action, string propertyName, string because = "", params object[] reasonArgs)
		{
			return action.ShouldThrow<AggregateRootIdPropertyNotFoundException>(because, reasonArgs)
				.Where(ex =>
					ex.PropertyName == propertyName &&
					ex.AggregateRootType == typeof(TAggregateRoot) &&
					ex.AggregateRootIdType == typeof(TAggregateRootId));
		}
	}
}
