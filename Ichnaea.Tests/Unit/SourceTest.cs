using System;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace Restall.Ichnaea.Tests.Unit
{
	public class SourceTest
	{
		[Fact]
		public void EventOf_CalledWithNullDomainEvent_ExpectNotImplementedException()
		{
			Action sourceNull = () => Source.Event.Of<object>(null);
			sourceNull.ShouldThrow<NotImplementedException>().Where(ExceptionMessageContainsFodyAndAggregate());
		}

		private static Expression<Func<NotImplementedException, bool>> ExceptionMessageContainsFodyAndAggregate()
		{
			return x => x.Message.Contains("Fody") && x.Message.Contains("[Aggregate]");
		}

		[Fact]
		public void EventOf_CalledWithNonNullDomainEvent_ExpectNotImplementedException()
		{
			Action sourceNonNull = () => Source.Event.Of(new object());
			sourceNonNull.ShouldThrow<NotImplementedException>().Where(ExceptionMessageContainsFodyAndAggregate());
		}
	}
}
