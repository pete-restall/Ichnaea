using System;
using FluentAssertions;

namespace Restall.Ichnaea.Fody.Tests.Integration
{
	public abstract class AggregateRootTest
	{
		protected readonly object AggregateRoot;

		protected AggregateRootTest(object aggregateRoot)
		{
			this.AggregateRoot = aggregateRoot;
		}

		protected void ExpectDynamicCallRaisesDomainEventWithSameToken(string eventFieldName, Action<dynamic, Guid> action)
		{
			var token = Guid.NewGuid();
			this.AggregateRoot.MonitorEvents();
			action(this.AggregateRoot, token);
			this.AggregateRoot.ShouldRaise(eventFieldName).WithDomainEvent<object>(this.AggregateRoot, x => SomethingHappenedWithToken(x, token));
		}

		protected static bool SomethingHappenedWithToken(object args, Guid token)
		{
			return args.GetType().Name == "SomethingHappened" && ((dynamic) args).Token == token;
		}
	}
}
