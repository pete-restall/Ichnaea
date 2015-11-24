using System;
using Mono.Cecil;

namespace Restall.Ichnaea.Fody
{
	public class ModuleWeaver
	{
		// TODO: Two events of different types - with Source.Event() call for both those exact types
		// TODO: Two events of the same type when Source.Event() used - weave neither (ambiguous), and output a build error
		// TODO: Two events of the same type when Source.Event() not used - no build error
		// TODO: Two events sourced with (cast) common base types and events declared with two common base types - should work due to explicit casts
		// TODO: Two events sourced with subtypes and events declared with two common base types - weave neither, ambiguous, so build error
		// TODO: One event sourced with two declared events with two common base types - weave neither, ambiguous, so build error
		// TODO: One event sourced (cast) with two declared events with two common base types - should work due to explicit cast
		// TODO: Multiple (heterogenous) event source replacements in the same method
		// TODO: Mixture of Source.Of<> and non-Source.Of<> events
		// TODO: Base of AggregateRoot declares the event, derived type sources the event
		// TODO: Async methods...
		// TODO: Source.Event() inside a closure
		// TODO: Derived type has event, base has no event.  Base calls Source.Event.Of() - should error
		// TODO: Abstract event
		// TODO: Abstract Aggregate Root with Event.SourceOf in the abstract class
		// TODO: Abstract Aggregate Root with Event.SourceOf in the concrete class
		public void Execute()
		{
			if (this.ModuleDefinition == null)
				throw new InvalidOperationException("ModuleDefinition cannot be null.  Is Fody integrated into the build correctly ?");

			foreach (var type in this.ModuleDefinition.GetTypes())
				new TypeWeaver(type).WeaveRaiseEventIntoAggregateRoot();
		}

		public ModuleDefinition ModuleDefinition { private get; set; }
	}
}
