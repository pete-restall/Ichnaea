using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NEventStore;
using NSubstitute;
using Restall.Ichnaea.NEventStore;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit.NEventStore
{
	public class NEventStoreSessionStreamTest
	{
		[Fact]
		public void Constructor_CalledWithNullEventStreamsInSession_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new NEventStoreSessionStream(null, DummyEventStream());
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventStreamsInSession");
		}

		private static IEventStream DummyEventStream()
		{
			return Substitute.For<IEventStream>();
		}

		[Fact]
		public void Constructor_CalledWithNullEventStream_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new NEventStoreSessionStream(DummyEventStreamsInSession(), null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventStream");
		}

		private static ConcurrentDictionary<Guid, IEventStream> DummyEventStreamsInSession()
		{
			return new ConcurrentDictionary<Guid, IEventStream>();
		}

		[Fact]
		public void Constructor_Called_ExpectStreamAddsItselfToTheSessionCollection()
		{
			var sessionCollection = new ConcurrentDictionary<Guid, IEventStream>();
			using (var stream = new NEventStoreSessionStream(sessionCollection, DummyEventStream()))
			{
				sessionCollection.Single().Value.Should().BeSameAs(stream);
			}
		}

		[Fact]
		public void Constructor_CalledMultipleTimes_ExpectStreamAddsItselfToTheSessionCollectionWithDifferentGuids()
		{
			var sessionCollection = new ConcurrentDictionary<Guid, IEventStream>();
			using (var firstStream = new NEventStoreSessionStream(sessionCollection, DummyEventStream()))
			{
				using (var secondStream = new NEventStoreSessionStream(sessionCollection, DummyEventStream()))
				{
					sessionCollection.Should().ContainValues(firstStream, secondStream);
				}
			}
		}

		[Fact]
		public void Dispose_Called_ExpectStreamRemovesItselfFromTheSessionCollection()
		{
			var sessionCollection = new ConcurrentDictionary<Guid, IEventStream>();
			var stream = new NEventStoreSessionStream(sessionCollection, DummyEventStream());
			stream.Dispose();
			sessionCollection.Should().BeEmpty();
		}

		[Fact]
		public void Dispose_CalledWheMultipleStreamsInTheSession_ExpectOnlyDisposedStreamIsRemovedFromTheSessionCollection()
		{
			var sessionCollection = new ConcurrentDictionary<Guid, IEventStream>();
			using (var nonDisposedStream = new NEventStoreSessionStream(sessionCollection, DummyEventStream()))
			{
				var stream = new NEventStoreSessionStream(sessionCollection, DummyEventStream());
				stream.Dispose();
				sessionCollection.Single().Value.Should().BeSameAs(nonDisposedStream);
			}
		}

		[Fact]
		public void Dispose_Called_ExpectDisposeOnEventStreamIsCalled()
		{
			var eventStream = MockEventStream();
			var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream);
			stream.Dispose();
			eventStream.Received(1).Dispose();
		}

		private static IEventStream MockEventStream()
		{
			return Substitute.For<IEventStream>();
		}

		[Fact]
		public void Dispose_CalledWhenEventStreamThrowsAnException_ExpectStreamRemovesItselfFromTheSessionCollection()
		{
			var eventStream = StubEventStream();
			eventStream.When(x => x.Dispose()).Do(_ => { throw new Exception(); });

			var sessionCollection = new ConcurrentDictionary<Guid, IEventStream>();
			var stream = new NEventStoreSessionStream(sessionCollection, eventStream);
			stream.Invoking(x => x.Dispose()).ShouldThrow<Exception>();

			sessionCollection.Should().NotContainValue(stream);
		}

		private static IEventStream StubEventStream()
		{
			return Substitute.For<IEventStream>();
		}

		[Fact]
		public void Add_CalledWithNullUncommittedEvent_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), DummyEventStream()))
			{
				stream.Invoking(x => x.Add(null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("uncommittedEvent");
			}
		}

		[Fact]
		public void Add_Called_ExpectCallIsProxiedToEventStream()
		{
			var eventStream = MockEventStream();
			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				var uncommittedEvent = new EventMessage();
				stream.Add(uncommittedEvent);
				eventStream.Received(1).Add(uncommittedEvent);
			}
		}

		[Fact]
		public void CommitChanges_Called_ExpectCallIsProxiedToEventStream()
		{
			var eventStream = MockEventStream();
			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				var commitId = Guid.NewGuid();
				stream.CommitChanges(commitId);
				eventStream.Received(1).CommitChanges(commitId);
			}
		}

		[Fact]
		public void ClearChanges_Called_ExpectCallIsProxiedToEventStream()
		{
			var eventStream = MockEventStream();
			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				stream.ClearChanges();
				eventStream.Received(1).ClearChanges();
			}
		}

		[Fact]
		public void BucketId_Get_ExpectSameValueAsEventStream()
		{
			var eventStream = StubEventStream();
			eventStream.BucketId.Returns(StringGenerator.AnyNonNull());

			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				stream.BucketId.Should().BeSameAs(eventStream.BucketId);
			}
		}

		[Fact]
		public void StreamId_Get_ExpectSameValueAsEventStream()
		{
			var eventStream = StubEventStream();
			eventStream.StreamId.Returns(StringGenerator.AnyNonNull());

			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				stream.StreamId.Should().BeSameAs(eventStream.StreamId);
			}
		}

		[Fact]
		public void StreamRevision_Get_ExpectSameValueAsEventStream()
		{
			var eventStream = StubEventStream();
			eventStream.StreamRevision.Returns(IntegerGenerator.Any());

			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				stream.StreamRevision.Should().Be(eventStream.StreamRevision);
			}
		}

		[Fact]
		public void CommitSequence_Get_ExpectSameValueAsEventStream()
		{
			var eventStream = StubEventStream();
			eventStream.CommitSequence.Returns(IntegerGenerator.Any());

			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				stream.CommitSequence.Should().Be(eventStream.CommitSequence);
			}
		}

		[Fact]
		public void CommittedEvents_Get_ExpectSameValueAsEventStream()
		{
			var eventStream = StubEventStream();
			eventStream.CommittedEvents.Returns(new EventMessage[0]);

			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				stream.CommittedEvents.Should().BeSameAs(eventStream.CommittedEvents);
			}
		}

		[Fact]
		public void CommittedHeaders_Get_ExpectSameValueAsEventStream()
		{
			var eventStream = StubEventStream();
			eventStream.CommittedHeaders.Returns(new Dictionary<string, object>());

			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				stream.CommittedHeaders.Should().BeSameAs(eventStream.CommittedHeaders);
			}
		}

		[Fact]
		public void UncommittedEvents_Get_ExpectSameValueAsEventStream()
		{
			var eventStream = StubEventStream();
			eventStream.UncommittedEvents.Returns(new EventMessage[0]);

			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				stream.UncommittedEvents.Should().BeSameAs(eventStream.UncommittedEvents);
			}
		}

		[Fact]
		public void UncommittedHeaders_Get_ExpectSameValueAsEventStream()
		{
			var eventStream = StubEventStream();
			eventStream.UncommittedHeaders.Returns(new Dictionary<string, object>());

			using (var stream = new NEventStoreSessionStream(DummyEventStreamsInSession(), eventStream))
			{
				stream.UncommittedHeaders.Should().BeSameAs(eventStream.UncommittedHeaders);
			}
		}
	}
}
