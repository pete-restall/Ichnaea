using System;
using Nancy;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo
{
	public class Bootstrapper: DefaultNancyBootstrapper
	{
		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			base.ConfigureRequestContainer(container, context);
			container.Register((ctx, _) => RouteRegistrarFactory.CreateDefaultInstance(ctx.Resolve));
		}

		protected override void ConfigureConventions(NancyConventions nancyConventions)
		{
			base.ConfigureConventions(nancyConventions);
			nancyConventions.ViewLocationConventions.Add((view, model, context) => ModelTypeToViewPath(((object) model).GetType()));
		}

		private static string ModelTypeToViewPath(Type modelType)
		{
			int lengthOfRootNamespaceAndDot = typeof(Bootstrapper).Namespace.Length + 1;
			var typeNameRelativeToRoot = modelType.FullName.Remove(0, lengthOfRootNamespaceAndDot);
			return typeNameRelativeToRoot.Replace('.', '/') + "View";
		}
	}
}
