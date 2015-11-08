using NEventStore;
using Nancy.TinyIoc;
using Restall.Ichnaea.NEventStore;

namespace Restall.Ichnaea.Demo.Web
{
	public static class IchnaeaBootstrapper
	{
		public static void RegisterRequestScopeIchnaeaDependenciesInto(TinyIoCContainer container)
		{
			container.Register(typeof(IDomainEventStream<,>), typeof(DomainEventStreamAdapter<,>)).AsMultiInstance();
			//container.Register(typeof(AggregateRootIdGetter<>), typeof(...));
			//container.Register(typeof(Converter<object, EventMessage>), typeof(...));
			// TODO: wire up dependencies
		}
	}
}
