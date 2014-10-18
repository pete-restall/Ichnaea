namespace Restall.Ichnaea.Demo.Web.Home
{
	public class HomeService
	{
		private readonly RouteLinks links;

		public HomeService(RouteLinks links)
		{
			this.links = links;
		}

		public IndexResponse Index(IndexRequest request)
		{
			return new IndexResponse { GetAllAccountsUri = this.links.Relative("GetAllAccounts") };
		}
	}
}
