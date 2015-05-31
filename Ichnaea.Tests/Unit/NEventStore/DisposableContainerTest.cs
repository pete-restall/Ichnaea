using System;
using System.Linq;
using NSubstitute;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit.NEventStore
{
	public class DisposableContainerTest
	{
		private class DisposableContainer: Ichnaea.NEventStore.DisposableContainer
		{
			public new void AddDisposable(IDisposable disposable)
			{
				base.AddDisposable(disposable);
			}
		}

		private static IDisposable[] MockAtLeastOneDisposable()
		{
			return SubstituteNumberOfDisposablesWithinExclusiveRange(1, 10);
		}

		private static IDisposable[] SubstituteNumberOfDisposablesWithinExclusiveRange(int min, int halfOpenMax)
		{
			int numberOfDisposables = IntegerGenerator.WithinExclusiveRange(min, halfOpenMax);
			return numberOfDisposables.Select(() => Substitute.For<IDisposable>()).ToArray();
		}

		[Fact]
		public void Dispose_Called_ExpectAllDisposablesAreDisposed()
		{
			var disposables = MockAtLeastOneDisposable();
			using (var container = new DisposableContainer())
			{
				disposables.ForEach(disposable => container.AddDisposable(disposable));
			}

			disposables.ForEach(disposable => disposable.Received(1).Dispose());
		}

		[Fact]
		public void Dispose_CalledMultipleTimes_ExpectDisposablesAreNotDisposedMoreThanOnce()
		{
			var disposables = MockAtLeastOneDisposable();
			using (var container = new DisposableContainer())
			{
				disposables.ForEach(disposable => container.AddDisposable(disposable));
				container.Dispose();
			}

			disposables.ForEach(disposable => disposable.Received(1).Dispose());
		}
	}
}
