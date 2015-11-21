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

# Not Production Ready...
Ichnaea is not (yet) ready for production.  While the core mechanisms are
in place and working, the library requires a lot more polishing and
edge-case implementation.  However, there is enough here to be of some use
to those who are interested, which is why I decided to share.

# Builds
[![Main CI](https://ci.appveyor.com/api/projects/status/ad199gnwd4lyc6wm)](https://ci.appveyor.com/project/pete-restall/ichnaea)
