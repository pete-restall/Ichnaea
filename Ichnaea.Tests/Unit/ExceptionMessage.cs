using System;

namespace Restall.Ichnaea.Tests.Unit
{
	public static class ExceptionMessage
	{
		public static string Default<T>() where T: Exception 
		{
			return "Exception of type '" + typeof(T) + "' was thrown.";
		}
	}
}
