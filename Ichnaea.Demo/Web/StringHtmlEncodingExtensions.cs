using System.Diagnostics.CodeAnalysis;
using Nancy.Helpers;
using Nancy.ViewEngines.Razor;
using NullGuard;

namespace Restall.Ichnaea.Demo.Web
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.UsedInView)]
	public static class StringHtmlEncodingExtensions
	{
		public static NonEncodedHtmlString InHtmlAttribute([AllowNull] this string str)
		{
			return new NonEncodedHtmlString(HttpUtility.HtmlAttributeEncode(str ?? string.Empty));
		}
	}
}
