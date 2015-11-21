using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Nancy;
using Restall.Nancy.ServiceRouting;

namespace Restall.Ichnaea.Demo.Web
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ReflectedByNancy)]
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
				return Assembly
					.GetExecutingAssembly()
					.GetTypes()
					.Where(x => x.IsClass && !x.IsAbstract && x.Namespace != null && x.Namespace.Contains(".Web") && x.Name.EndsWith("Service"));
			}
		}
	}
}
