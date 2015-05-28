using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NEventStore;
using NSubstitute;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit.NEventStore
{
	public class PersistedEventStreamContainerTest
	{
		private class PersistedEventStreamContainer: Ichnaea.NEventStore.PersistedEventStreamContainer
		{
			public new void AddStream(IEventStream stream)
			{
				base.AddStream(stream);
			}
		}

		[Fact]
		public void Commit_CalledWithNoExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithNonEmptyId()
		{
			var streams = MockAtLeastOneEventStoreStream();
			using (var container = new PersistedEventStreamContainer())
			{
				streams.ForEach(stream => container.AddStream(stream));
				container.Commit();
				streams.ForEach(stream => stream.Received(1).CommitChanges(Arg.Is<Guid>(commitId => commitId != Guid.Empty)));
			}
		}

		private static IEventStream[] MockAtLeastOneEventStoreStream()
		{
			return SubstituteNumberOfEventStoreStreamsWithinExclusiveRange(1, 10);
		}

		private static IEventStream[] SubstituteNumberOfEventStoreStreamsWithinExclusiveRange(int min, int halfOpenMax)
		{
			int numberOfEventStoreStreams = IntegerGenerator.WithinExclusiveRange(min, halfOpenMax);
			return numberOfEventStoreStreams.Select(() => Substitute.For<IEventStream>()).ToArray();
		}

		[Fact]
		public void Commit_CalledWithNoExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithSameId()
		{
			var commitIds = new List<Guid>();
			var streams = StubAtLeastTwoEventStoreStreams();
			streams.ForEach(es => es.CommitChanges(Arg.Do<Guid>(commitIds.Add)));
			using (var container = new PersistedEventStreamContainer())
			{
				streams.ForEach(stream => container.AddStream(stream));
				container.Commit();
				commitIds.Distinct().Count().Should().Be(1);
			}
		}

		private static IEventStream[] StubAtLeastTwoEventStoreStreams()
		{
			return SubstituteNumberOfEventStoreStreamsWithinExclusiveRange(2, 10);
		}

		[Fact]
		public void Commit_CalledMultipleTimesWithNoExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithDifferentIdsPerCall()
		{
			var commitIds = new List<Guid>();
			var streams = StubAtLeastTwoEventStoreStreams();
			streams.ForEach(es => es.CommitChanges(Arg.Do<Guid>(commitIds.Add)));
			using (var container = new PersistedEventStreamContainer())
			{
				streams.ForEach(stream => container.AddStream(stream));
				int numberOfCommits = IntegerGenerator.WithinExclusiveRange(2, 10);
				numberOfCommits.Repeat(container.Commit);
				commitIds.Distinct().Count().Should().Be(numberOfCommits);
			}
		}

		[Fact]
		public void Commit_CalledWithExplicitCommitId_ExpectAllEventStoreStreamsAreCommittedWithSameId()
		{
			var streams = MockAtLeastOneEventStoreStream();
			using (var container = new PersistedEventStreamContainer())
			{
				streams.ForEach(stream => container.AddStream(stream));
				var commitId = Guid.NewGuid();
				container.Commit(commitId);
				streams.ForEach(stream => stream.Received(1).CommitChanges(commitId));
			}
		}

		[Fact]
		public void ClearUncommitted_Called_ExpectAllEventStoreStreamsAreCleared()
		{
			var streams = MockAtLeastOneEventStoreStream();
			using (var container = new PersistedEventStreamContainer())
			{
				streams.ForEach(stream => container.AddStream(stream));
				container.ClearUncommitted();
				streams.ForEach(stream => stream.Received(1).ClearChanges());
			}
		}

		[Fact]
		public void Dispose_Called_ExpectAllEventStoreStreamsAreDisposed()
		{
			var streams = MockAtLeastOneEventStoreStream();
			using (var container = new PersistedEventStreamContainer())
			{
				streams.ForEach(stream => container.AddStream(stream));
			}

			streams.ForEach(stream => stream.Received(1).Dispose());
		}

		[Fact]
		public void Dispose_CalledMultipleTimes_ExpectEventStoreStreamsAreNotDisposedMoreThanOnce()
		{
			var streams = MockAtLeastOneEventStoreStream();
			using (var container = new PersistedEventStreamContainer())
			{
				streams.ForEach(stream => container.AddStream(stream));
				container.Dispose();
			}

			streams.ForEach(stream => stream.Received(1).Dispose());
		}
	}
}
