using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Demo.Web.Home
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ReflectedByNancyServiceRouting)]
	public class HomeService
	{
		private readonly RouteLinks links;

		public HomeService(RouteLinks links)
		{
			this.links = links;
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = CodeAnalysisJustification.ReflectedByNancyServiceRouting)]
		public IndexResponse Index(IndexRequest request)
		{
			return new IndexResponse { GetAllAccountsUri = this.links.Relative(RouteNames.GetAllAccounts) };
		}
	}
}
