using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Events;

namespace Restall.Ichnaea.Fody.Tests
{
	public static class AggregateRootDomainEventMonitorExtensions
	{
		[SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "Null check is done by Fluent Assertions")]
		public static void MonitorDomainEvent<TDomainEvent>(this object aggregateRoot, string eventFieldName)
		{
			if (aggregateRoot == null)
				throw new ArgumentNullException(nameof(aggregateRoot));

			var eventInfo = aggregateRoot.GetType().GetEvent(eventFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			eventInfo.Should().NotBeNull("because event {0} should be declared in type {1}.", eventFieldName, aggregateRoot.GetType());

			var eventRecorder = new EventRecorder(aggregateRoot, eventInfo.Name);
			var eventHandler = new Source.Of<TDomainEvent>((sender, args) => eventRecorder.RecordEvent(sender, args));
			eventInfo.GetAddMethod(true).Invoke(aggregateRoot, new object[] {eventHandler});
			EventMonitor.AddRecordersFor(aggregateRoot, obj => new[] {eventRecorder});
		}
	}
}
