using NSubstitute;

namespace Restall.Ichnaea.Tests
{
	public static class PrePersistenceDomainEventTrackerTestDoubles
	{
		private class DummyPrePersistenceDomainEventTrackerThatWillNotHoldOntoReferences: IPrePersistenceDomainEventTracker<object>
		{
			public void SwitchTrackingToPersistentStore(object aggregateRoot, Source.Of<object> persistentObserver)
			{
			}
		}

		public static IPrePersistenceDomainEventTracker<object> Dummy()
		{
			return new DummyPrePersistenceDomainEventTrackerThatWillNotHoldOntoReferences();
		}

		public static IPrePersistenceDomainEventTracker<object> Mock()
		{
			return Stub();
		}

		public static IPrePersistenceDomainEventTracker<object> Stub()
		{
			return Substitute.For<IPrePersistenceDomainEventTracker<object>>();
		}

		public static IPrePersistenceDomainEventTracker<object> Spy()
		{
			return Stub();
		}
	}
}
