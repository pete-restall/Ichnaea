using System;
using FluentAssertions;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public abstract class AggregateTest
	{
		protected readonly object Aggregate;

		protected AggregateTest(object aggregate)
		{
			this.Aggregate = aggregate;
		}

		protected void ExpectDynamicCallRaisesDomainEventWithSameToken(string eventFieldName, Action<dynamic, Guid> action)
		{
			var token = Guid.NewGuid();
			this.Aggregate.MonitorEvents();
			action(this.Aggregate, token);
			this.Aggregate.ShouldRaise(eventFieldName).WithDomainEvent<object>(this.Aggregate, x => SomethingHappenedWithToken(x, token));
		}

		protected static bool SomethingHappenedWithToken(object args, Guid token)
		{
			return args.GetType().Name == "SomethingHappened" && ((dynamic) args).Token == token;
		}
	}
}
