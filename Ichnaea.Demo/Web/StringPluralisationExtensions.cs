namespace Restall.Ichnaea.Demo.Web
{
	public static class StringPluralisationExtensions
	{
		public static string Pluralise(this string singular)
		{
			if (singular.Length == 0)
				return string.Empty;

			return singular + 's';
		}
	}
}
