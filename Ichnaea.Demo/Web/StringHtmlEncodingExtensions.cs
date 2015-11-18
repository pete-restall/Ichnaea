using Nancy.Helpers;
using Nancy.ViewEngines.Razor;
using NullGuard;

namespace Restall.Ichnaea.Demo.Web
{
	public static class StringHtmlEncodingExtensions
	{
		public static NonEncodedHtmlString InHtmlAttribute([AllowNull] this string str)
		{
			return new NonEncodedHtmlString(HttpUtility.HtmlAttributeEncode(str ?? string.Empty));
		}
	}
}
