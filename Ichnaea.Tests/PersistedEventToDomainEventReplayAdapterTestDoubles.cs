using System;
using System.Collections.Generic;
using NEventStore;
using NSubstitute;
using Restall.Ichnaea.NEventStore;

namespace Restall.Ichnaea.Tests
{
	public static class PersistedEventToDomainEventReplayAdapterTestDoubles
	{
		private class DummyReplayAdapterThatWillNotHoldOntoReferences: PersistedEventToDomainEventReplayAdapter<object>
		{
			public DummyReplayAdapterThatWillNotHoldOntoReferences(): base(DummyConverter, DummyDomainEventReplay())
			{
			}

			public override bool CanReplay(object aggregateRoot, EventMessage persistedEvent)
			{
				return true;
			}

			public override object Replay(object aggregateRoot, EventMessage persistedEvent)
			{
				return new object();
			}
		}

		private static object DummyConverter(EventMessage persistedEvent)
		{
			return new object();
		}

		private static IReplayDomainEvents<object> DummyDomainEventReplay()
		{
			return Substitute.For<IReplayDomainEvents<object>>();
		}

		public static PersistedEventToDomainEventReplayAdapter<object> Dummy()
		{
			return new DummyReplayAdapterThatWillNotHoldOntoReferences();
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

		public static PersistedEventToDomainEventReplayAdapter<object> Stub()
		{
			return Substitute.For<PersistedEventToDomainEventReplayAdapter<object>>(
				new Converter<EventMessage, object>(DummyConverter),
				DummyDomainEventReplay());
		}
	}
}
