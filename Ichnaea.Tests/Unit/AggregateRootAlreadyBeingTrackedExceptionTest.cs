namespace Restall.Ichnaea.Tests.Unit
{
	using ExceptionTest = ExceptionTest<AggregateRootAlreadyBeingTrackedException, AggregateRootAlreadyBeingTrackedExceptionTest.DefaultExceptionProperties>;

	public class AggregateRootAlreadyBeingTrackedExceptionTest: ExceptionTest
	{
		public new class ExceptionProperties: ExceptionTest.ExceptionProperties
		{
		}

		public class DefaultExceptionProperties: ExceptionProperties
		{
			public DefaultExceptionProperties()
			{
				this.Message = ExceptionMessage.Default<AggregateRootAlreadyBeingTrackedException>();
			}
		}
	}
}
