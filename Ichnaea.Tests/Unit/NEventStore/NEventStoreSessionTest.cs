using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using NEventStore;
using NEventStore.Persistence;
using NSubstitute;
using Restall.Ichnaea.NEventStore;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit.NEventStore
{
	public class NEventStoreSessionTest
	{
		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullEventStore_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new NEventStoreSession(null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("eventStore");
		}

		[Fact]
		public void CreateStream_CalledWithNullBucketId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var session = new NEventStoreSession(DummyEventStore()))
			{
				session.Invoking(x => x.CreateStream(null, DummyStreamId())).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("bucketId");
			}
		}

		private static IStoreEvents DummyEventStore()
		{
			return Substitute.For<IStoreEvents>();
		}

		private static string DummyStreamId()
		{
			return StringGenerator.AnyNonNull();
		}

		[Fact]
		public void CreateStream_CalledWithNullStreamId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var session = new NEventStoreSession(DummyEventStore()))
			{
				session.Invoking(x => x.CreateStream(DummyBucketId(), null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("streamId");
			}
		}

		private static string DummyBucketId()
		{
			return StringGenerator.AnyNonNull();
		}

		[Fact]
		public void CreateStream_Called_ExpectCallIsProxiedToEventStoreCreateStream()
		{
			var eventStream = MockEventStream();
			var bucketId = DummyBucketId();
			var streamId = DummyStreamId();
			var eventStore = StubEventStore();
			eventStore.CreateStream(bucketId, streamId).Returns(eventStream);

			using (var session = new NEventStoreSession(eventStore))
			{
				var eventMessage = new EventMessage();
				session.CreateStream(bucketId, streamId).Add(eventMessage);
				eventStream.Received(1).Add(eventMessage);
			}
		}

		private static IEventStream MockEventStream()
		{
			return Substitute.For<IEventStream>();
		}

		private static IStoreEvents StubEventStore()
		{
			return Substitute.For<IStoreEvents>();
		}

		[Fact]
		public void CreateStream_Called_ExpectNEventStoreSessionStreamInstanceIsReturned()
		{
			var eventStore = StubEventStore();
			eventStore.CreateStream(Arg.Any<string>(), Arg.Any<string>()).Returns(DummyEventStream());

			using (var session = new NEventStoreSession(eventStore))
			{
				session.CreateStream(DummyBucketId(), DummyStreamId()).Should().BeOfType<NEventStoreSessionStream>();
			}
		}

		private static IEventStream DummyEventStream()
		{
			return Substitute.For<IEventStream>();
		}

		[Fact]
		public void OpenStream_CalledWithNullBucketId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var session = new NEventStoreSession(DummyEventStore()))
			{
				session.Invoking(x => x.OpenStream(null, DummyStreamId(), AnyRevision(), AnyRevision()))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("bucketId");
			}
		}

		private static int AnyRevision()
		{
			return IntegerGenerator.Any();
		}

		[Fact]
		public void OpenStream_CalledWithNullStreamId_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var session = new NEventStoreSession(DummyEventStore()))
			{
				session.Invoking(x => x.OpenStream(DummyBucketId(), null, AnyRevision(), AnyRevision()))
					.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("streamId");
			}
		}

		[Fact]
		public void OpenStream_CalledWithBucketAndStreamIds_ExpectCallIsProxiedToEventStoreOpenStream()
		{
			var eventStream = MockEventStream();
			var bucketId = DummyBucketId();
			var streamId = DummyStreamId();
			var minRevision = AnyRevision();
			var maxRevision = AnyRevision();
			var eventStore = StubEventStore();
			eventStore.OpenStream(bucketId, streamId, minRevision, maxRevision).Returns(eventStream);

			using (var session = new NEventStoreSession(eventStore))
			{
				var eventMessage = new EventMessage();
				session.OpenStream(bucketId, streamId, minRevision, maxRevision).Add(eventMessage);
				eventStream.Received(1).Add(eventMessage);
			}
		}

		[Fact]
		public void OpenStream_CalledWithBucketAndStreamIds_ExpectNEventStoreSessionStreamInstanceIsReturned()
		{
			var eventStore = StubEventStore();
			eventStore.OpenStream(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>()).Returns(DummyEventStream());

			using (var session = new NEventStoreSession(eventStore))
			{
				session.OpenStream(DummyBucketId(), DummyStreamId(), AnyRevision(), AnyRevision()).Should().BeOfType<NEventStoreSessionStream>();
			}
		}

		[Fact]
		public void OpenStream_CalledWithNullSnapshot_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			using (var session = new NEventStoreSession(DummyEventStore()))
			{
				session.Invoking(x => x.OpenStream(null, AnyRevision())).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("snapshot");
			}
		}

		[Fact]
		public void OpenStream_CalledWithSnapshot_ExpectCallIsProxiedToEventStoreOpenStream()
		{
			var eventStream = MockEventStream();
			var snapshot = DummySnapshot();
			var maxRevision = AnyRevision();
			var eventStore = StubEventStore();
			eventStore.OpenStream(snapshot, maxRevision).Returns(eventStream);

			using (var session = new NEventStoreSession(eventStore))
			{
				var eventMessage = new EventMessage();
				session.OpenStream(snapshot, maxRevision).Add(eventMessage);
				eventStream.Received(1).Add(eventMessage);
			}
		}

		private static ISnapshot DummySnapshot()
		{
			return Substitute.For<ISnapshot>();
		}

		[Fact]
		public void OpenStream_CalledWithSnapshot_ExpectNEventStoreSessionStreamInstanceIsReturned()
		{
			var eventStore = StubEventStore();
			eventStore.OpenStream(Arg.Any<ISnapshot>(), Arg.Any<int>()).Returns(DummyEventStream());

			using (var session = new NEventStoreSession(eventStore))
			{
				session.OpenStream(DummySnapshot(), AnyRevision()).Should().BeOfType<NEventStoreSessionStream>();
			}
		}

		[Fact]
		public void StartDispatchScheduler_Called_ExpectCallIsProxiedToEventStoreStartDispatchScheduler()
		{
			var eventStore = MockEventStore();
			using (var session = new NEventStoreSession(eventStore))
			{
				session.StartDispatchScheduler();
				eventStore.Received(1).StartDispatchScheduler();
			}
		}

		private static IStoreEvents MockEventStore()
		{
			return Substitute.For<IStoreEvents>();
		}

		[Fact]
		public void Dispose_Called_ExpectCreatedStreamsAreDisposed()
		{
			var eventStreams = MockAtLeastOneEventStream();
			var eventStore = StubEventStoreForCreateStreams(eventStreams);

			using (var session = new NEventStoreSession(eventStore))
			{
				eventStreams.ForEach(_ => session.CreateStream(DummyBucketId(), DummyStreamId()));
			}

			eventStreams.ForEach(stream => stream.Received(1).Dispose());
		}

		private static IEventStream[] MockAtLeastOneEventStream()
		{
			return SubstituteNumberOfEventStoreStreamsWithinExclusiveRange(1, 10);
		}

		private static IEventStream[] SubstituteNumberOfEventStoreStreamsWithinExclusiveRange(int min, int halfOpenMax)
		{
			int numberOfEventStoreStreams = IntegerGenerator.WithinExclusiveRange(min, halfOpenMax);
			return numberOfEventStoreStreams.Select(() => Substitute.For<IEventStream>()).ToArray();
		}

		private static IStoreEvents StubEventStoreForCreateStreams(IEventStream[] eventStreams)
		{
			var eventStore = StubEventStore();
			eventStore.CreateStream(Arg.Any<string>(), Arg.Any<string>()).Returns(eventStreams.First(), eventStreams.Skip(1).ToArray());
			return eventStore;
		}

		[Fact]
		public void Dispose_Called_ExpectStreamsOpenedWithBucketAndStreamIdsAreDisposed()
		{
			var eventStreams = MockAtLeastOneEventStream();
			var eventStore = StubEventStore();
			eventStore.OpenStream(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
				.Returns(eventStreams.First(), eventStreams.Skip(1).ToArray());

			using (var session = new NEventStoreSession(eventStore))
			{
				eventStreams.ForEach(_ => session.OpenStream(DummyBucketId(), DummyStreamId(), AnyRevision(), AnyRevision()));
			}

			eventStreams.ForEach(stream => stream.Received(1).Dispose());
		}

		[Fact]
		public void Dispose_Called_ExpectStreamsOpenedWithSnapshotsAreDisposed()
		{
			var eventStreams = MockAtLeastOneEventStream();
			var eventStore = StubEventStore();
			eventStore.OpenStream(Arg.Any<ISnapshot>(), Arg.Any<int>()).Returns(eventStreams.First(), eventStreams.Skip(1).ToArray());

			using (var session = new NEventStoreSession(eventStore))
			{
				eventStreams.ForEach(_ => session.OpenStream(DummySnapshot(), AnyRevision()));
			}

			eventStreams.ForEach(stream => stream.Received(1).Dispose());
		}

		[Fact]
		public void Dispose_CalledMultipleTimes_ExpectEventStreamsAreNotDisposedMoreThanOnce()
		{
			var eventStreams = MockAtLeastOneEventStream();
			var eventStore = StubEventStoreForCreateStreams(eventStreams);
			using (var session = new NEventStoreSession(eventStore))
			{
				eventStreams.ForEach(_ => session.CreateStream(DummyBucketId(), DummyStreamId()));
				session.Dispose();
			}

			eventStreams.ForEach(stream => stream.Received(1).Dispose());
		}

		[Fact]
		public void Commit_CalledWithNoExplicitCommitId_ExpectAllEventStreamsAreCommittedWithNonEmptyId()
		{
			var eventStreams = MockAtLeastOneEventStream();
			var eventStore = StubEventStoreForCreateStreams(eventStreams);
			using (var session = new NEventStoreSession(eventStore))
			{
				eventStreams.ForEach(_ => session.CreateStream(DummyBucketId(), DummyStreamId()));
				session.Commit();
				eventStreams.ForEach(stream => stream.Received(1).CommitChanges(Arg.Is<Guid>(commitId => commitId != Guid.Empty)));
			}
		}

		[Fact]
		public void Commit_CalledWithNoExplicitCommitId_ExpectAllEventStreamsAreCommittedWithSameId()
		{
			var commitIds = new List<Guid>();
			var eventStreams = StubAtLeastTwoEventStreams();
			eventStreams.ForEach(stream => stream.CommitChanges(Arg.Do<Guid>(commitIds.Add)));

			var eventStore = StubEventStoreForCreateStreams(eventStreams);
			using (var session = new NEventStoreSession(eventStore))
			{
				eventStreams.ForEach(_ => session.CreateStream(DummyBucketId(), DummyStreamId()));
				session.Commit();
				commitIds.Distinct().Count().Should().Be(1);
			}
		}

		private static IEventStream[] StubAtLeastTwoEventStreams()
		{
			return SubstituteNumberOfEventStoreStreamsWithinExclusiveRange(2, 10);
		}

		[Fact]
		public void Commit_CalledMultipleTimesWithNoExplicitCommitId_ExpectAllEventStreamsAreCommittedWithDifferentIdsPerCall()
		{
			var commitIds = new List<Guid>();
			var eventStreams = StubAtLeastTwoEventStreams();
			eventStreams.ForEach(stream => stream.CommitChanges(Arg.Do<Guid>(commitIds.Add)));

			var eventStore = StubEventStoreForCreateStreams(eventStreams);
			using (var session = new NEventStoreSession(eventStore))
			{
				eventStreams.ForEach(_ => session.CreateStream(DummyBucketId(), DummyStreamId()));
				int numberOfCommits = IntegerGenerator.WithinExclusiveRange(2, 10);
				numberOfCommits.Repeat(session.Commit);
				commitIds.Distinct().Count().Should().Be(numberOfCommits);
			}
		}

		[Fact]
		public void Commit_CalledWithExplicitCommitId_ExpectAllEventStreamsAreCommittedWithSameId()
		{
			var eventStreams = MockAtLeastOneEventStream();
			var eventStore = StubEventStoreForCreateStreams(eventStreams);
			using (var session = new NEventStoreSession(eventStore))
			{
				eventStreams.ForEach(_ => session.CreateStream(DummyBucketId(), DummyStreamId()));
				var commitId = Guid.NewGuid();
				session.Commit(commitId);
				eventStreams.ForEach(stream => stream.Received(1).CommitChanges(commitId));
			}
		}

		[Fact]
		public void ClearUncommitted_Called_ExpectAllEventStreamsAreCleared()
		{
			var eventStreams = MockAtLeastOneEventStream();
			var eventStore = StubEventStoreForCreateStreams(eventStreams);
			using (var session = new NEventStoreSession(eventStore))
			{
				eventStreams.ForEach(_ => session.CreateStream(DummyBucketId(), DummyStreamId()));
				session.ClearUncommitted();
				eventStreams.ForEach(stream => stream.Received(1).ClearChanges());
			}
		}

		[Fact]
		public void Advanced_Get_ExpectSameValueAsEventStore()
		{
			var advanced = DummyAdvanced();
			var eventStore = StubEventStore();
			eventStore.Advanced.Returns(advanced);

			using (var session = new NEventStoreSession(eventStore))
			{
				session.Advanced.Should().BeSameAs(advanced);
			}
		}

		private static IPersistStreams DummyAdvanced()
		{
			return Substitute.For<IPersistStreams>();
		}
	}
}
