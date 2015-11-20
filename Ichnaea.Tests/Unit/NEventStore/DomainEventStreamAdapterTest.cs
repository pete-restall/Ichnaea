using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NEventStore;
using NSubstitute;
using Restall.Ichnaea.NEventStore;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit.NEventStore
{
	public class DomainEventStreamAdapterTest
	{
		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullPersistedEventStreamCreator_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStreamAdapter<object, string>(null, DummyPersistedEventStreamOpener());
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("persistedEventStreamCreator");
		}

		private static PersistedEventStreamOpener<object, string> DummyPersistedEventStreamOpener()
		{
			return SubstitutePersistedEventStreamOpener();
		}

		private static PersistedEventStreamOpener<object, string> SubstitutePersistedEventStreamOpener()
		{
			return Substitute.For<PersistedEventStreamOpener<object, string>>(
				Substitute.For<IStoreEvents>(),
				StringGenerator.AnyNonNull(),
				new Converter<string, string>(x => StringGenerator.AnyNonNull()),
				PostPersistenceDomainEventTrackerTestDoubles.Dummy(),
				new Converter<object, EventMessage>(x => new EventMessage()),
				PersistedEventToDomainEventReplayAdapterTestDoubles.Dummy());
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullPersistedEventStreamOpener_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventStreamAdapter<object, string>(DummyPersistedEventStreamCreator(), null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("persistedEventStreamOpener");
		}

		private static PersistedEventStreamCreator<object> DummyPersistedEventStreamCreator()
		{
			return SubstitutePersistedEventStreamCreator();
		}

		private static PersistedEventStreamCreator<object> SubstitutePersistedEventStreamCreator()
		{
			return Substitute.For<PersistedEventStreamCreator<object>>(
				Substitute.For<IStoreEvents>(),
				StringGenerator.AnyNonNull(),
				PrePersistenceDomainEventTrackerTestDoubles.Dummy(),
				new AggregateRootIdGetter<object>(x => StringGenerator.AnyNonNull()),
				new Converter<object, EventMessage>(x => new EventMessage()));
		}

		[Fact]
		public void CreateFrom_CalledWithNullAggregateRoot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var adapter = new DomainEventStreamAdapter<object, string>(DummyPersistedEventStreamCreator(), DummyPersistedEventStreamOpener());
			adapter.Invoking(x => x.CreateFrom(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRoot");
		}

		[Fact]
		public void CreateFrom_Called_ExpectCallIsProxiedToPersistedEventStreamCreator()
		{
			using (var streamCreator = MockPersistedEventStreamCreator())
			{
				var adapter = new DomainEventStreamAdapter<object, string>(streamCreator, DummyPersistedEventStreamOpener());
				var aggregateRoot = new object();
				adapter.CreateFrom(aggregateRoot);
				streamCreator.Received(1).CreateFrom(aggregateRoot);
			}
		}

		private static PersistedEventStreamCreator<object> MockPersistedEventStreamCreator()
		{
			return SubstitutePersistedEventStreamCreator();
		}

		[Fact]
		public void Replay_CalledWithNullAggregateRootId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			var adapter = new DomainEventStreamAdapter<object, string>(DummyPersistedEventStreamCreator(), DummyPersistedEventStreamOpener());
			adapter.Invoking(x => x.Replay(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("aggregateRootId");
		}

		[Fact]
		public void Replay_Called_ExpectCallIsProxiedToPersistedEventStreamOpener()
		{
			using (var streamOpener = StubPersistedEventStreamOpener())
			{
				var aggregateRootId = StringGenerator.AnyNonNull();
				var aggregateRoot = new object();
				streamOpener.Replay(aggregateRootId).Returns(aggregateRoot);

				var adapter = new DomainEventStreamAdapter<object, string>(DummyPersistedEventStreamCreator(), streamOpener);
				adapter.Replay(aggregateRootId).Should().BeSameAs(aggregateRoot);
			}
		}

		private static PersistedEventStreamOpener<object, string> StubPersistedEventStreamOpener()
		{
			return SubstitutePersistedEventStreamOpener();
		}
	}
}
