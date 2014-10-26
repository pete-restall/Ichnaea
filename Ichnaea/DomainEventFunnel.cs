using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restall.Ichnaea
{
	public class DomainEventFunnel: IDisposable
	{
		private readonly WeakReference<object> observable;
		private readonly Delegate observer;

		public DomainEventFunnel(object observable, Source.Of<object> observer)
		{
			this.observable = new WeakReference<object>(observable);
			this.observer = observer;

			GetDomainEventAddMethodsFrom(observable).ForEach(e => e.Invoke(observable, new object[] {observer}));
		}

		private static IEnumerable<MethodInfo> GetDomainEventAddMethodsFrom(object observable)
		{
			return GetDomainEventsFrom(observable).Select(x => x.GetAddMethod(true));
		}

		private static IEnumerable<EventInfo> GetDomainEventsFrom(object observable)
		{
			return GetAllInstanceEventsFrom(observable)
				.Where(x => x.EventHandlerType.GetGenericTypeDefinition() == typeof(Source.Of<>));
		}

		private static IEnumerable<EventInfo> GetAllInstanceEventsFrom(object observable)
		{
			return observable.GetType().GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public void Dispose()
		{
			object strongReference;
			if (!this.observable.TryGetTarget(out strongReference))
				return;

			GetDomainEventRemoveMethodsFrom(strongReference).ForEach(e => e.Invoke(strongReference, new object[] {observer}));
		}

		private static IEnumerable<MethodInfo> GetDomainEventRemoveMethodsFrom(object observable)
		{
			return GetDomainEventsFrom(observable).Select(x => x.GetRemoveMethod(true));
		}
	}
}
