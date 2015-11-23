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
			var token = this.InvokeTokenActionOnMonitoredAggregateRoot(action);
			this.AggregateRoot.ShouldRaise(eventFieldName).WithDomainEvent<object>(this.AggregateRoot, x => SomethingHappenedWithToken(x, token));
		}

		private Guid InvokeTokenActionOnMonitoredAggregateRoot(Action<dynamic, Guid> action)
		{
			var token = Guid.NewGuid();
			this.AggregateRoot.MonitorEvents();
			action(this.AggregateRoot, token);
			return token;
		}

		protected static bool SomethingHappenedWithToken(object args, Guid token)
		{
			return args.GetType().Name == "SomethingHappened" && ((dynamic) args).Token == token;
		}

		protected void ExpectDynamicCallRaisesDomainEventWithSameObjectInitialisedToken(string eventFieldName, Action<dynamic, Guid> action)
		{
			var token = this.InvokeTokenActionOnMonitoredAggregateRoot(action);
			this.AggregateRoot
				.ShouldRaise(eventFieldName)
				.WithDomainEvent<object>(this.AggregateRoot, x => ObjectInitialiserSomethingHappenedWithToken(x, token));
		}

		private static bool ObjectInitialiserSomethingHappenedWithToken(object args, Guid token)
		{
			return args.GetType().Name == "ObjectInitialiserSomethingHappened" && ((dynamic) args).Token == token;
		}
	}
}
