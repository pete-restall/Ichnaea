# What is this ?
[Ichnaea](https://github.com/pete-restall/Ichnaea)
is a .NET framework that provides unobtrusive tracking for
[Aggregate Roots](http://martinfowler.com/bliki/DDD_Aggregate.html).
It achieves this by leveraging native .NET Events to provide a more natural
paradigm for an
[Event Sourcing](http://docs.geteventstore.com/introduction/event-sourcing-basics/)
implementation.

# Why use this ?
Event Sourcing is a powerful solution to the age-old problem of persisting
long-lived objects with some very interesting (beneficial) side-effects.  In
the .NET world this problem is typically solved with an Object Relational
Mapper (ORM) such as Entity Framework, or by hand-rolling a Data Access Layer
(DAL).  Both of these techniques operate on snapshots of _current_ state, and
introduce other problems that quickly add complexity into the application and
cloud the expressiveness of the Model with persistence concerns.

A big source of complexity that can be removed by Event Sourcing is the
mapping of objects into a relational representation.  Mapping often forces
compromises in the Model design such as the duplication or flattening of
object hierarchies, or the introduction of serialisation constraints.  It
is also often difficult to change the shape of the objects when new
requirements dictate changing the persisted objects.

# What Problem does Ichnaea Solve ?
Most Event Sourcing libraries and discussions centre around the Event Store
mechanism rather than how the Domain Events are Sourced and passed to that
Event Store.  This neglected 'bit in the middle' can actually introduce
significant complexity or detract from the expressiveness of the Model,
thus negating some of the benefits of an Event Sourcing approach over
traditional state-based persistence.  Ichnaea is my attempt at providing
this boilerplate in as unobtrusive manner as possible.

# Motivation
A common approach to Event Sourcing that I've seen involves the
Aggregate Roots keeping track of their own Domain Events (ie. a list) so
that Repositories can query them when it is time to persist.  To me this
approach, while simple, just gives an Aggregate Root another responsibility
and detracts from the core purpose of the Model by adding what are effectively
persistence concerns.

A few years ago I implemented a .NET Event-based solution for a client as
I wished to avoid this list-based approach.  While I was happy with the
natural fit of native .NET Events that allowed a more pure Aggregate Root
representation, I was unhappy that the complexity had just been shifted
into the Repositories.  I realised a more generic approach could be
built but the project time constraints did not allow this.  It was an
interesting enough problem for me to decide to solve it in my own time as
it ought to help with future projects.

# The Demo
The [demo](https://github.com/pete-restall/Ichnaea/Ichnaea.Demo/) is an
overly simplified Domain Model that consists of an
[Account](https://github.com/pete-restall/Ichnaea/Ichnaea.Demo/Accounts/Account.cs)
Aggregate Root.

## The Aggregate Root
To enable [Ichnaea.Fody](https://github.com/pete-restall/Ichnaea/Fody/Ichnaea.Fody)
integration it is decorated with the
[AggregateRoot](https://github.com/pete-restall/Ichnaea/Ichnaea/AggregateRootAttribute.cs)
attribute, which will automatically replace stub Event Sourcing calls with
boilerplate methods to raise corresponding (declared) .NET Events.

The [Fody](https://github.com/Fody/Fody) plugin provides an easily readable
and idiomatic way to Source Events without the repetitive clutter of manually
rolled .NET Event raising code, but it is not a required dependency if you
would rather raise the Events manually:
```C#
    [AggregateRoot]
    public class Account
    {
        ...

        public void Credit(decimal amount, string description)
        {
            EnsureAmountIsPositiveOrZero(amount);

            this.Balance += amount;
            Source.Event.Of(new BalanceCredited(amount, description));
        }

        ...

        public event Source.Of<AccountEvent> EventSource;
    }
```
## The Repository
The
[AccountRepository](https://github.com/pete-restall/Ichnaea/Ichnaea.Demo/Accounts/AccountRepository.cs)
is not cluttered with CRUD or mapping concerns, simply
providing query methods for the
[Account](https://github.com/pete-restall/Ichnaea/Ichnaea.Demo/Accounts/Account.cs)
Aggregate Roots in a collection-like
manner.  The
[IDomainEventStream<,>](https://github.com/pete-restall/Ichnaea/Ichnaea/IDomainEventStream.cs)
dependency provides the Ichnaea abstraction to leverage the tracking and Event Storing:
```C#
    public class AccountRepository
    {
        private readonly IDomainEventStream<Account, AccountId> stream;

        public AccountRepository(IDomainEventStream<Account, AccountId> stream)
        {
            this.stream = stream;
        }

        public void Add(Account account)
        {
            this.stream.CreateFrom(account);
        }

        public Account GetBySortCodeAndAccountNumber(string sortCode, string accountNumber)
        {
            return this.stream.Replay(new AccountId(sortCode, accountNumber));
        }
    }
```
## The Factory
While Ichnaea *implicitly* tracks Domain Events from Aggregate Roots that
have been retrieved from a Repository via an Event Stream, it needs to be
*explicitly* told of newly created Aggregate Roots before it can track them.
Rather than 'newing up' an Aggregate Root it is recommended that the Factory
pattern is leveraged to encapsulate this concern, ie.
[AccountFactory](https://github.com/pete-restall/Ichnaea/Ichnaea.Demo/Accounts/AccountFactory.cs):
```C#
    public class AccountFactory
    {
        private readonly IDomainEventTracker<Account> tracker;

        public AccountFactory(IDomainEventTracker<Account> tracker)
        {
            this.tracker = tracker;
        }

        public Account Create(string sortCode, string accountNumber, string holder)
        {
            var account = new Account(new AccountId(sortCode, accountNumber), holder);
            this.tracker.AggregateRootCreated(account, new AccountOpened(account.Id, holder));
            return account;
        }
    }
```
The
[IDomainEventTracker<>](https://github.com/pete-restall/Ichnaea/Ichnaea/IDomainEventTracker.cs)
dependency provides the Ichnaea abstraction for tracking Domain Events
until the Aggregate Root has been added into a Repository (ie. persisted).

## Bootstrapping
Ichnaea needs wiring up, such as in the example
[IchnaeaBootstrapper.cs](https://github.com/pete-restall/Ichnaea/Ichnaea.Demo/Web/IchnaeaBootstrapper.cs).
Currently this is Poor Man's DI, but proper fluent encapsulation is a
requirement before v1.0.0.

Ichnaea currently only supports
[NEventStore](https://github.com/NEventStore/NEventStore) as a persistence
mechanism but it can easily be extended.

# Not Production Ready...
Ichnaea is not (yet) ready for production.  While the core mechanisms are
in place and working, the library requires a lot more polishing and
edge-case implementation.  However, there is enough here to be of some use
to those who are interested, which is why I decided to share.

# Builds
[![Main CI](https://ci.appveyor.com/api/projects/status/22147m70ih66dw8i)](https://ci.appveyor.com/project/pete-restall/ichnaea)

