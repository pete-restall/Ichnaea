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

		protected void ExpectDynamicCallSourcesDomainEventWithSameToken(string eventFieldName, Action<dynamic, Guid> action)
		{
			var token = this.InvokeTokenActionOnMonitoredAggregateRoot(eventFieldName, action);
			this.AggregateRoot
				.ShouldRaise(eventFieldName)
				.WithDomainEvent<object>(this.AggregateRoot, x => SomethingHappenedWithToken(x, token));
		}

		private Guid InvokeTokenActionOnMonitoredAggregateRoot(string eventFieldName, Action<dynamic, Guid> action)
		{
			return this.InvokeTokenActionOnMonitoredAggregateRoot<object>(eventFieldName, action);
		}

		protected Guid InvokeTokenActionOnMonitoredAggregateRoot<TDomainEvent>(string eventFieldName, Action<dynamic, Guid> action)
		{
			this.AggregateRoot.MonitorDomainEvent<TDomainEvent>(eventFieldName);
			var token = Guid.NewGuid();
			action(this.AggregateRoot, token);
			return token;
		}

		protected static bool SomethingHappenedWithToken(object args, Guid token)
		{
			return EventRaisedWithToken("SomethingHappened", args, token);
		}

		private static bool EventRaisedWithToken(string eventName, object args, Guid token)
		{
			var eventType = args.GetType();
			var tokenProperty = eventType.GetProperty("Token");
			return eventType.Name == eventName && tokenProperty != null && tokenProperty.GetValue(args).Equals(token);
		}

		protected void ExpectDynamicCallSourcesDomainEventWithSameObjectInitialisedToken(string eventFieldName, Action<dynamic, Guid> action)
		{
			var token = this.InvokeTokenActionOnMonitoredAggregateRoot(eventFieldName, action);
			this.AggregateRoot
				.ShouldRaise(eventFieldName)
				.WithDomainEvent<object>(this.AggregateRoot, x => EventRaisedWithToken("ObjectInitialiserSomethingHappened", x, token));
		}

		protected void ExpectDynamicCallSourcesDerivedDomainEventWithSameToken(string eventFieldName, Action<dynamic, Guid> action)
		{
			var token = this.InvokeTokenActionOnMonitoredAggregateRoot(eventFieldName, action);
			this.AggregateRoot
				.ShouldRaise(eventFieldName)
				.WithDomainEvent<object>(this.AggregateRoot, x => EventRaisedWithToken("SomethingDerivedHappened", x, token));
		}
	}
}
