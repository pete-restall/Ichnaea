using System.Collections.Generic;
using System.Linq;
using NullGuard;

namespace Restall.Ichnaea
{
	public class DomainEventReplayChain<TAggregateRoot>: IReplayDomainEvents<TAggregateRoot> where TAggregateRoot: class
	{
		private readonly IReplayDomainEvents<TAggregateRoot>[] replayChain;

		public DomainEventReplayChain(IEnumerable<IReplayDomainEvents<TAggregateRoot>> replayChain)
			: this(replayChain.ToArray())
		{
		}

		public DomainEventReplayChain(params IReplayDomainEvents<TAggregateRoot>[] replayChain)
		{
			this.replayChain = replayChain;
		}

		public bool CanReplay([AllowNull] TAggregateRoot aggregateRoot, [AllowNull] object domainEvent)
		{
			return replayChain.Any(x => x.CanReplay(aggregateRoot, domainEvent));
		}

		[return: AllowNull]
		public TAggregateRoot Replay([AllowNull] TAggregateRoot aggregateRoot, [AllowNull] object domainEvent)
		{
			var replays = this.replayChain.Where(x => x.CanReplay(aggregateRoot, domainEvent)).ToArray();
			if (replays.Length != 1)
			{
				throw new DomainEventCannotBeReplayedException(
					"Could not replay Domain Event because there are " + replays.Length + " matching replays; " +
						"aggregateRoot=" + aggregateRoot +
						", domainEvent=" + domainEvent,
					domainEvent);
			}

			return replays[0].Replay(aggregateRoot, domainEvent);
		}
	}
}
