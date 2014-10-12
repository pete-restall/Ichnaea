using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nancy;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo
{
	public class ServiceModule: NancyModule
	{
		public ServiceModule(RouteRegistrar routes)
		{
			routes.RegisterServicesInto(this, AllServiceTypes);
		}

		private static IEnumerable<Type> AllServiceTypes
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.Name.EndsWith("Service"));
			}
		}
	}
}
