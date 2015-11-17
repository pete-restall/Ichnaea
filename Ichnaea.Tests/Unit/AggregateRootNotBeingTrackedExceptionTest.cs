namespace Restall.Ichnaea.Tests.Unit
{
	using ExceptionTest = ExceptionTest<AggregateRootNotBeingTrackedException, AggregateRootNotBeingTrackedExceptionTest.DefaultExceptionProperties>;

	public class AggregateRootNotBeingTrackedExceptionTest: ExceptionTest
	{
		public new class ExceptionProperties: ExceptionTest.ExceptionProperties
		{
		}

		public class DefaultExceptionProperties: ExceptionProperties
		{
			public DefaultExceptionProperties()
			{
				this.Message = ExceptionMessage.Default<AggregateRootNotBeingTrackedException>();
			}
		}
	}
}
