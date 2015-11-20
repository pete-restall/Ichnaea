using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = Ichnaea.CodeAnalysisJustification.IchnaeaSubscribes)]
	public class AggregateRootWithTwoDomainEvents
	{
		public void SourceFirstDomainEvent(object domainEvent)
		{
			this.SourceEvent(this.FirstEvent, domainEvent);
		}

		private void SourceEvent(Source.Of<object> eventField, object domainEvent)
		{
			eventField?.Invoke(this, domainEvent);
		}

		public void SourceBothDomainEvents(object firstDomainEvent, object secondDomainEvent)
		{
			this.SourceEvent(this.FirstEvent, firstDomainEvent);
			this.SourceEvent(this.SecondEvent, secondDomainEvent);
		}

		public event Source.Of<object> FirstEvent;
		public event Source.Of<object> SecondEvent;
	}
}
