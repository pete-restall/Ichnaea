using System;
using System.Diagnostics.CodeAnalysis;
using NullGuard;

namespace Restall.Ichnaea
{
	public static class Source
	{
		public class EventFluency
		{
			[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = CodeAnalysisJustification.RequiredByWeaver)]
			[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global", Justification = CodeAnalysisJustification.RequiredByWeaver)]
			public void Of<T>([AllowNull] T domainEvent)
			{
				throw new NotImplementedException(
					"Domain Events should only be Sourced by Aggregate Roots.  Ichnaea will only weave calls to " +
					"Source.Event.Of() if all of the following conditions are met:\n" +
					"\t- Your Aggregate Root class is decorated with the [AggregateRoot] attribute\n" +
					"\t- The call to Source.Event.Of() is in an instance method of the Aggregate Root\n" +
					"\t- The Aggregate Root has a .NET event field using the Source.Of<> delegate, whose type argument unambiguously " +
						"corresponds to the Domain Event's type\n" +
					"This exception can also be thrown if the Ichnaea.Fody plugin was not integrated into the build correctly.");
			}
		}

		public static readonly EventFluency Event = new EventFluency();

		public delegate void Of<in T>(object aggregateRoot, T domainEvent);
	}
}
