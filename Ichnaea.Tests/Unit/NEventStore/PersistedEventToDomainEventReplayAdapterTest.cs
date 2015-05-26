using System;
using FluentAssertions;
using NEventStore;
using NSubstitute;
using Restall.Ichnaea.NEventStore;
using Xunit;
using Xunit.Extensions;

namespace Restall.Ichnaea.Tests.Unit.NEventStore
{
	public class PersistedEventToDomainEventReplayAdapterTest
	{
		[Fact]
		public void Constructor_CalledWithNullPersistedEventToDomainEventConverter_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventToDomainEventReplayAdapter<object>(null, DummyEventReplay());
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("persistedEventToDomainEventConverter");
		}

		private static IReplayDomainEvents<object> DummyEventReplay()
		{
			return Substitute.For<IReplayDomainEvents<object>>();
		}

		[Fact]
		public void Constructor_CalledWithNullDomainEventReplay_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new PersistedEventToDomainEventReplayAdapter<object>(DummyEventConverter, null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("domainEventReplay");
		}

		private static object DummyEventConverter(EventMessage eventMessage)
		{
			return new object();
		}

		[Theory]
		[InlineData(true), InlineData(false)]
		public void CanReplay_CalledWithNullAggregateRoot_ExpectCallToDomainEventCanReplayWithNullAggregateRootAndConvertedDomainEvent(bool canReplay)
		{
			CanReplay_Called_ExpectCallToDomainEventCanReplayWithSameAggregateRootAndConvertedDomainEvent(null, canReplay);
		}

		private static void CanReplay_Called_ExpectCallToDomainEventCanReplayWithSameAggregateRootAndConvertedDomainEvent(object aggregateRoot, bool canReplay)
		{
			var domainEvent = DummyDomainEvent();
			var replay = StubDomainEventReplay();
			replay.CanReplay(aggregateRoot, domainEvent).Returns(canReplay);

			var persistedEvent = DummyPersistedEvent();
			var eventConverter = StubEventConverter(persistedEvent, domainEvent);
			var adapter = new PersistedEventToDomainEventReplayAdapter<object>(eventConverter, replay);
			adapter.CanReplay(aggregateRoot, persistedEvent).Should().Be(canReplay);
		}

		private static object DummyDomainEvent()
		{
			return new object();
		}

		private static IReplayDomainEvents<object> StubDomainEventReplay()
		{
			return Substitute.For<IReplayDomainEvents<object>>();
		}

		private static EventMessage DummyPersistedEvent()
		{
			return new EventMessage();
		}

		private static Converter<EventMessage, object> StubEventConverter(EventMessage persistedEvent, object domainEvent)
		{
			var converter = Substitute.For<Converter<EventMessage, object>>();
			converter(persistedEvent).Returns(domainEvent);
			return converter;
		}

		[Theory]
		[InlineData(true), InlineData(false)]
		public void CanReplay_CalledWithNonNullAggregateRoot_ExpectCallToDomainEventCanReplayWithSameAggregateRootAndConvertedDomainEvent(bool canReplay)
		{
			CanReplay_Called_ExpectCallToDomainEventCanReplayWithSameAggregateRootAndConvertedDomainEvent(DummyAggregateRoot(), canReplay);
		}

		private static object DummyAggregateRoot()
		{
			return new object();
		}

		[Fact]
		public void Replay_CalledWithNullAggregateRoot_ExpectCallToDomainEventReplayWithNullAggregateRootAndConvertedDomainEvent()
		{
			Replay_Called_ExpectCallToDomainEventReplayWithSameAggregateRootAndConvertedDomainEvent(null);
		}

		private static void Replay_Called_ExpectCallToDomainEventReplayWithSameAggregateRootAndConvertedDomainEvent(object aggregateRoot)
		{
			var domainEvent = DummyDomainEvent();
			var newAggregateRoot = DummyAggregateRoot();
			var replay = StubDomainEventReplay();
			replay.Replay(aggregateRoot, domainEvent).Returns(newAggregateRoot);

			var persistedEvent = DummyPersistedEvent();
			var eventConverter = StubEventConverter(persistedEvent, domainEvent);
			var adapter = new PersistedEventToDomainEventReplayAdapter<object>(eventConverter, replay);
			adapter.Replay(aggregateRoot, persistedEvent).Should().BeSameAs(newAggregateRoot);
		}

		[Fact]
		public void Replay_CalledWithNonNullAggregateRoot_ExpectCallToDomainEventReplayWithSameAggregateRootAndConvertedDomainEvent()
		{
			Replay_Called_ExpectCallToDomainEventReplayWithSameAggregateRootAndConvertedDomainEvent(DummyAggregateRoot());
		}
	}
}
