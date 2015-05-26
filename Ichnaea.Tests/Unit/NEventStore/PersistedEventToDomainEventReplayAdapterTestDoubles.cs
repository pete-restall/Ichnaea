using System;
using System.Collections.Generic;
using NEventStore;
using NSubstitute;
using Restall.Ichnaea.NEventStore;

namespace Restall.Ichnaea.Tests.Unit.NEventStore
{
	public static class PersistedEventToDomainEventReplayAdapterTestDoubles
	{
		public static PersistedEventToDomainEventReplayAdapter<object> Dummy()
		{
			var replay = Stub();
			replay.CanReplay(Arg.Any<object>(), Arg.Any<EventMessage>()).Returns(true);
			replay.Replay(Arg.Any<object>(), Arg.Any<EventMessage>()).Returns(new object());
			return replay;
		}

		public static PersistedEventToDomainEventReplayAdapter<object> Stub()
		{
			return Substitute.For<PersistedEventToDomainEventReplayAdapter<object>>(
				new Converter<EventMessage, object>(DummyConverter),
				DummyDomainEventReplay());
		}

		private static object DummyConverter(EventMessage persistedEvent)
		{
			return new object();
		}

		private static IReplayDomainEvents<object> DummyDomainEventReplay()
		{
			return Substitute.For<IReplayDomainEvents<object>>();
		}

		public static PersistedEventToDomainEventReplayAdapter<object> StubForCommittedEventSequence(
			IReadOnlyList<EventMessage> committedEvents, IList<object> aggregateRootUnderConstruction)
		{
			var replay = Stub();
			committedEvents.Count.Repeat(i =>
				{
					var previousAggregation = i == 0 ? null : aggregateRootUnderConstruction[i - 1];
					replay.CanReplay(previousAggregation, committedEvents[i]).Returns(true);
					replay.Replay(previousAggregation, committedEvents[i]).Returns(aggregateRootUnderConstruction[i]);
				});

			return replay;
		}
	}
}
