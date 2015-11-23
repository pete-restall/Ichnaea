using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	public class SourceEventFromStaticProperty
	{
		private static Guid tokenForPropertyGet;

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public object DoSomethingInPropertyGet(Guid token)
		{
			tokenForPropertyGet = token;
			return Property;
		}

		private static Guid Property
		{
			get
			{
				Source.Event.Of(new SomethingHappened(tokenForPropertyGet));
				return tokenForPropertyGet;
			}

			set
			{
				Source.Event.Of(new SomethingHappened(value));
			}
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public void DoSomethingInPropertySet(Guid token)
		{
			Property = token;
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
