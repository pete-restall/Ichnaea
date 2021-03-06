﻿using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class DomainEventFunnelTest
	{
		private class HasEventConvenienceMethods
		{
			protected static void SourceEvent<T>(Source.Of<T> eventDelegate, object sender, T args)
			{
				eventDelegate?.Invoke(sender, args);
			}

			protected static void RaiseEvent<T>(EventHandler<T> eventDelegate, object sender, T args)
			{
				eventDelegate?.Invoke(sender, args);
			}
		}

		private class ObservableWithOneDomainEvent: HasEventConvenienceMethods
		{
			public void SourceFirstEvent(object sender, object args)
			{
				SourceEvent(this.FirstEvent, sender, args);
			}

			[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public event Source.Of<object> FirstEvent;
		}

		private class ObservableWithTwoDomainEvents: ObservableWithOneDomainEvent
		{
			public void SourceSecondEvent(object sender, object args)
			{
				SourceEvent(this.SecondEvent, sender, args);
			}

			[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public event Source.Of<object> SecondEvent;
		}

		private class ObservableWithNonPublicDomainEvents: HasEventConvenienceMethods
		{
			public void SourcePrivateEvent(object sender, object args)
			{
				SourceEvent(this.PrivateEvent, sender, args);
			}

			[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			private event Source.Of<object> PrivateEvent;

			public void SourceProtectedEvent(object sender, object args)
			{
				SourceEvent(this.ProtectedEvent, sender, args);
			}

			[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			protected event Source.Of<object> ProtectedEvent;

			public void SourceInternalEvent(object sender, object args)
			{
				SourceEvent(this.InternalEvent, sender, args);
			}

			[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			internal event Source.Of<object> InternalEvent;

			public void SourceProtectedInternalEvent(object sender, object args)
			{
				SourceEvent(this.ProtectedInternalEvent, sender, args);
			}

			[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			protected internal event Source.Of<object> ProtectedInternalEvent;
		}

		private class ObservableWithNonDomainEvent: HasEventConvenienceMethods
		{
			public void RaiseFirstEvent(object sender, EventArgs args)
			{
				RaiseEvent(this.FirstEvent, sender, args);
			}

			[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public event EventHandler<EventArgs> FirstEvent;
		}

		private class ObservableWithStaticDomainEvent: HasEventConvenienceMethods
		{
			[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public void SourceFirstEvent(object sender, object args)
			{
				SourceEvent(FirstEvent, sender, args);
			}

			[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Local", Justification = CodeAnalysisJustification.StubForTesting)]
			public static event Source.Of<object> FirstEvent;
		}

		// TODO: Base class events should also be funnelled - I think non-public base-class events won't be returned via Type.GetEvents(), much like non-public fields / properties...

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullObservable_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventFunnel(null, (sender, args) => { });
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("observable");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = CodeAnalysisJustification.TestingConstructorException)]
		public void Constructor_CalledWithNullObserver_ExpectArgumentNullExceptionWithCorrectParamName()
		{
			Action constructor = () => new DomainEventFunnel(new object(), null);
			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("observer");
		}

		[Fact]
		public void Constructor_CalledWhenObservableHasOneEvent_ExpectTheSingleEventIsSubscribedTo()
		{
			ExpectObserverIsCalledForSourcedEvent<ObservableWithOneDomainEvent>(
				(observer, sender, args) => observer.SourceFirstEvent(sender, args));
		}

		private static void ExpectObserverIsCalledForSourcedEvent<T>(Action<T, object, object> sourceEvent) where T: new()
		{
			var observer = Substitute.For<Source.Of<object>>();
			var observable = new T();
			using (new DomainEventFunnel(observable, observer))
			{
				var sender = new object();
				var args = new object();
				sourceEvent(observable, sender, args);
				observer.Received(1).Invoke(sender, args);
			}
		}

		[Fact]
		public void Constructor_CalledWhenObservableHasMultipleEvents_ExpectAllEventsAreSubscribedTo()
		{
			var observer = Substitute.For<Source.Of<object>>();
			var observable = new ObservableWithTwoDomainEvents();
			using (new DomainEventFunnel(observable, observer))
			{
				var sender = new[] {new object(), new object()};
				var args = new[] {new object(), new object()};
				observable.SourceFirstEvent(sender[0], args[0]);
				observable.SourceSecondEvent(sender[1], args[1]);

				observer.Received(1).Invoke(sender[0], args[0]);
				observer.Received(1).Invoke(sender[1], args[1]);
			}
		}

		[Fact]
		public void Constructor_CalledWhenObservableHasPrivateEvent_ExpectThePrivateEventIsSubscribedTo()
		{
			ExpectObserverIsCalledForSourcedEvent<ObservableWithNonPublicDomainEvents>(
				(observer, sender, args) => observer.SourcePrivateEvent(sender, args));
		}

		[Fact]
		public void Constructor_CalledWhenObservableHasProtectedEvent_ExpectTheProtectedEventIsSubscribedTo()
		{
			ExpectObserverIsCalledForSourcedEvent<ObservableWithNonPublicDomainEvents>(
				(observer, sender, args) => observer.SourceProtectedEvent(sender, args));
		}

		[Fact]
		public void Constructor_CalledWhenObservableHasInternalEvent_ExpectTheInternalEventIsSubscribedTo()
		{
			ExpectObserverIsCalledForSourcedEvent<ObservableWithNonPublicDomainEvents>(
				(observer, sender, args) => observer.SourceInternalEvent(sender, args));
		}

		[Fact]
		public void Constructor_CalledWhenObservableHasProtectedInternalEvent_ExpectTheProtectedInternalEventIsSubscribedTo()
		{
			ExpectObserverIsCalledForSourcedEvent<ObservableWithNonPublicDomainEvents>(
				(observer, sender, args) => observer.SourceProtectedInternalEvent(sender, args));
		}

		[Fact]
		public void Constructor_CalledWhenObservableHasNonDomainEvent_ExpectTheNonDomainEventIsNotSubscribedTo()
		{
			var observer = Substitute.For<Source.Of<object>>();
			var observable = new ObservableWithNonDomainEvent();
			using (new DomainEventFunnel(observable, observer))
			{
				var sender = new object();
				var args = new EventArgs();
				observable.RaiseFirstEvent(sender, args);
				observer.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<object>());
			}
		}

		[Fact]
		public void Constructor_CalledWhenObservableHasStaticDomainEvent_ExpectTheStaticDomainEventIsNotSubscribedTo()
		{
			var observer = Substitute.For<Source.Of<object>>();
			var observable = new ObservableWithStaticDomainEvent();
			using (new DomainEventFunnel(observable, observer))
			{
				var sender = new object();
				var args = new object();
				observable.SourceFirstEvent(sender, args);
				observer.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<object>());
			}
		}

		[Fact]
		public void Dispose_CalledWhenObservableHasOneEvent_ExpectTheSingleEventIsUnsubscribedFrom()
		{
			ExpectObserverIsNotCalledForSourcedEventAfterFunnelDisposed<ObservableWithOneDomainEvent>(
				(observer, sender, args) => observer.SourceFirstEvent(sender, args));
		}

		private static void ExpectObserverIsNotCalledForSourcedEventAfterFunnelDisposed<T>(Action<T, object, object> sourceEvent) where T: new()
		{
			var observer = Substitute.For<Source.Of<object>>();
			var observable = new T();
			var funnel = new DomainEventFunnel(observable, observer);
			funnel.Dispose();
			sourceEvent(observable, new object(), new object());
			observer.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<object>());
		}

		[Fact]
		public void Dispose_CalledWhenObservableHasMultipleEvents_ExpectAllEventsAreUnsubscribedFrom()
		{
			var observer = Substitute.For<Source.Of<object>>();
			var observable = new ObservableWithTwoDomainEvents();
			var funnel = new DomainEventFunnel(observable, observer);
			funnel.Dispose();
			observable.SourceFirstEvent(new object(), new object());
			observable.SourceSecondEvent(new object(), new object());
			observer.DidNotReceive().Invoke(Arg.Any<object>(), Arg.Any<object>());
		}

		[Fact]
		public void Dispose_CalledWhenObservableHasPrivateEvent_ExpectThePrivateEventIsUnsubscribedFrom()
		{
			ExpectObserverIsNotCalledForSourcedEventAfterFunnelDisposed<ObservableWithNonPublicDomainEvents>(
				(observer, sender, args) => observer.SourcePrivateEvent(sender, args));
		}

		[Fact]
		public void Dispose_CalledWhenObservableHasProtectedEvent_ExpectTheProtectedEventIsUnsubscribedFrom()
		{
			ExpectObserverIsNotCalledForSourcedEventAfterFunnelDisposed<ObservableWithNonPublicDomainEvents>(
				(observer, sender, args) => observer.SourceProtectedEvent(sender, args));
		}

		[Fact]
		public void Dispose_CalledWhenObservableHasInternalEvent_ExpectTheInternalEventIsUnsubscribedFrom()
		{
			ExpectObserverIsNotCalledForSourcedEventAfterFunnelDisposed<ObservableWithNonPublicDomainEvents>(
				(observer, sender, args) => observer.SourceInternalEvent(sender, args));
		}

		[Fact]
		public void Dispose_CalledWhenObservableHasProtectedInternalEvent_ExpectTheProtectedInternalEventIsUnsubscribedFrom()
		{
			ExpectObserverIsNotCalledForSourcedEventAfterFunnelDisposed<ObservableWithNonPublicDomainEvents>(
				(observer, sender, args) => observer.SourceProtectedInternalEvent(sender, args));
		}

		[Fact]
		public void Dispose_CalledWhenObservableHasBeenGarbageCollected_ExpectNoException()
		{
			WeakReference<ObservableWithOneDomainEvent> weakObservable;
			var funnel = CreateDomainEventFunnelWithWeakObservable(out weakObservable);
			Collect.Garbage();

			ObservableWithOneDomainEvent observable;
			weakObservable.TryGetTarget(out observable).Should().BeFalse("because the observable should have been garbage collected");
			funnel.Invoking(x => x.Dispose()).ShouldNotThrow();
		}

		private static DomainEventFunnel CreateDomainEventFunnelWithWeakObservable(out WeakReference<ObservableWithOneDomainEvent> weakObservable)
		{
			var observable = new ObservableWithOneDomainEvent();
			weakObservable = new WeakReference<ObservableWithOneDomainEvent>(observable);
			return new DomainEventFunnel(observable, Substitute.For<Source.Of<object>>());
		}

		[Fact]
		public void Dispose_CalledMultipleTimes_ExpectNoException()
		{
			var funnel = new DomainEventFunnel(new ObservableWithOneDomainEvent(), Substitute.For<Source.Of<object>>());
			funnel.Dispose();
			funnel.Invoking(x => x.Dispose()).ShouldNotThrow();
		}
	}
}
