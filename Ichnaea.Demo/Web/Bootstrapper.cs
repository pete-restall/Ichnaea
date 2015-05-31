using System;
using System.Collections.Generic;
using System.Reflection;
using Nancy;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo.Web
{
	public class Bootstrapper: DefaultNancyBootstrapper
	{
		protected override IEnumerable<Func<Assembly, bool>> AutoRegisterIgnoredAssemblies
		{
			get { return new Func<Assembly, bool>[] {asm => asm.GetName().Name != "Restall.Ichnaea.Demo"}; }
		}

		protected override void ConfigureApplicationContainer(TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer(container);
			container.Register((ctx, args) => RouteRegistrarFactory.CreateDefaultInstance(ctx.Resolve));
			DatabaseBootstrapper.RegisterApplicationScopeDatabaseDependenciesInto(container);
		}

		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			base.ConfigureRequestContainer(container, context);
			container.Register((ctx, args) => context);
			DatabaseBootstrapper.RegisterRequestScopeDatabaseDependenciesInto(container);
		}

		protected override void ConfigureConventions(NancyConventions nancyConventions)
		{
			base.ConfigureConventions(nancyConventions);
			nancyConventions.ViewLocationConventions.Add((view, model, context) => ModelTypeToViewPath(((object) model).GetType()));
		}

		private static string ModelTypeToViewPath(Type modelType)
		{
			var typeNameRelativeToRoot = modelType.FullName.Remove(0, "Restall.Ichnaea.Demo.".Length);
			return typeNameRelativeToRoot.Replace('.', '/') + "View";
		}
	}
}
