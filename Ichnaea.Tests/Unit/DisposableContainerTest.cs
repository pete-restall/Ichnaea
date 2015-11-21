using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class DisposableContainerTest
	{
		private class DisposableContainer: Ichnaea.DisposableContainer
		{
			public new void AddDisposable(IDisposable disposable)
			{
				base.AddDisposable(disposable);
			}

			public new void RemoveDisposable(IDisposable disposable)
			{
				base.RemoveDisposable(disposable);
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
		[SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = CodeAnalysisJustification.EnumerableIsMaterialisedBeforeDisposal)]
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
		[SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = CodeAnalysisJustification.EnumerableIsMaterialisedBeforeDisposal)]
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

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = CodeAnalysisJustification.EnumerableIsMaterialisedBeforeDisposal)]
		public void Dispose_CalledWhenSomeDisposablesHaveBeenRemoved_ExpectOnlyRemainingDisposablesAreDisposed()
		{
			var remainingDisposables = MockAnyNumberOfDisposables();
			var removedDisposables = MockAtLeastOneDisposable();
			var allDisposables = remainingDisposables.Concat(removedDisposables).Shuffle();
			using (var container = new DisposableContainer())
			{
				allDisposables.ForEach(disposable => container.AddDisposable(disposable));
				removedDisposables.ForEach(disposable => container.RemoveDisposable(disposable));
			}

			remainingDisposables.ForEach(disposable => disposable.Received(1).Dispose());
			removedDisposables.ForEach(disposable => disposable.Received(0).Dispose());
		}

		private static IDisposable[] MockAnyNumberOfDisposables()
		{
			return SubstituteNumberOfDisposablesWithinExclusiveRange(0, 10);
		}
	}
}
