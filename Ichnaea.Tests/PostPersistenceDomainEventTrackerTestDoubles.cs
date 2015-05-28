namespace Restall.Ichnaea.Tests
{
	public static class PostPersistenceDomainEventTrackerTestDoubles
	{
		private class DummyPostPersistenceDomainEventTrackerThatWillNotHoldOntoReferences: IPostPersistenceDomainEventTracker<object>
		{
			public void TrackToPersistentStore(object aggregateRoot, Source.Of<object> persistentObserver)
			{
			}
		}

		public static IPostPersistenceDomainEventTracker<object> Dummy()
		{
			return new DummyPostPersistenceDomainEventTrackerThatWillNotHoldOntoReferences();
		}
	}
}
