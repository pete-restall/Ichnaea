using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	public class SourceEventFromInstanceProperty
	{
		private Guid tokenForPropertyGet;

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public object DoSomethingInPropertyGet(Guid token)
		{
			this.tokenForPropertyGet = token;
			return this.Property;
		}

		private Guid Property
		{
			get
			{
				Source.Event.Of(new SomethingHappened(this.tokenForPropertyGet));
				return this.tokenForPropertyGet;
			}

			set
			{
				Source.Event.Of(new SomethingHappened(value));
			}
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
		public void DoSomethingInPropertySet(Guid token)
		{
			this.Property = token;
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
