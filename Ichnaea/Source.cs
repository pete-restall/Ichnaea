using System;

namespace Restall.Ichnaea
{
	public static class Source
	{
		public class EventFluency
		{
			public void Of<T>(T domainEvent)
			{
				throw new NotImplementedException(
					"Domain Events should only be Sourced by Aggregates.  Ichnaea will only weave calls to " +
					"Source.Event.Of() if all of the following conditions are met:\n" +
					"\t- Your Aggregate class is decorated with the [Aggregate] attribute\n" +
					"\t- The call to Source.Event.Of() is in an instance method of the Aggregate\n" +
					"\t- The Aggregate has a .NET event field using the Source.Of<> delegate, whose type argument corresponds to the Domain Event's type\n" +
					"This exception can also be thrown if the Ichnaea.Fody plugin was not integrated into the build correctly.");
			}
		}

		public static readonly EventFluency Event = new EventFluency();

		public delegate void Of<in T>(object aggregate, T domainEvent);
	}
}
