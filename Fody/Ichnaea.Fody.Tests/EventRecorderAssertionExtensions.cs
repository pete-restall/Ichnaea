using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions.Events;
using FluentAssertions.Execution;

namespace Restall.Ichnaea.Fody.Tests
{
	public static class EventRecorderAssertionExtensions
	{
		public static void WithDomainEvent<T>(
			this IEventRecorder eventRecorder,
			object aggregate,
			Expression<Func<T, bool>> predicate,
			string because = "",
			params object[] reasonArgs)
		{
			var matchingPredicate = predicate.Compile();
			var events = eventRecorder
				.Select(x => new { Args = x.Parameters.ToArray() })
				.Where(x => x.Args.Length == 2 && ReferenceEquals(x.Args[0], aggregate));

			Execute.Assertion
				.ForCondition(events.Select(x => x.Args[1]).OfType<T>().Any(matchingPredicate))
				.BecauseOf(because, reasonArgs)
				.FailWith("Expected Domain Event of type {0} matching {1}{reason}", typeof(T), predicate);
		}
	}
}
