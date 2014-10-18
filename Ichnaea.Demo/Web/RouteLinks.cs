using System;
using Nancy;
using Nancy.Linker;

namespace Restall.Ichnaea.Demo.Web
{
	public class RouteLinks
	{
		private readonly NancyContext context;
		private readonly IResourceLinker linker;

		public RouteLinks(NancyContext context, IResourceLinker linker)
		{
			this.context = context;
			this.linker = linker;
		}

		public Uri Relative(string routeName, dynamic parameters = null)
		{
			return this.linker.BuildRelativeUri(this.context, routeName, parameters);
		}
	}
}
