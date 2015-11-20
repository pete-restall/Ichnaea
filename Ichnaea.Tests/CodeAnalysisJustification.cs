using System.ComponentModel;

namespace Restall.Ichnaea.Tests
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class CodeAnalysisJustification
	{
		public const string DynamicProxyRequiresPublicTypes = "Castle.DynamicProxy requires public types";
		public const string ParticipatesInPartialMocking = "Participates in partial mocking";
		public const string ReflectivePropertyComparison = "Property is reflected as part of object comparisons";
		public const string StubForTesting = "Stub for testing";
		public const string TestingConstructorException = "Testing constructor exception";
		public const string TestingMultipleEnumerations = "Testing against multiple enumerations";
		public const string TestingNullBehaviour = "Testing null behaviour";
	}
}
