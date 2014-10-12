using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Restall.Ichnaea.Fody
{
	public class ModuleWeaver
	{
		// TODO: Calling Source.Event(new XXX { Prop1 = xxx, Prop2 = yyy, ... }) - (ie. where does the ldarg.0 go ?)
		// TODO: Calling Source.Event(someMethodCall()) - (ie. where does the ldarg.0 go ?)
		// TODO: Calling Source.Event(this.someField) - (ie. where does the ldarg.0 go ?)
		// TODO: Calling Source.Event(this.someProperty) - (ie. where does the ldarg.0 go ?)
		// TODO: Two events of different types - with Source.Event() call for both those exact types
		// TODO: Two events of the same type when Source.Event() used - weave neither (ambiguous), and output a build error
		// TODO: Two events of the same type when Source.Event() not used - no build error
		// TODO: Event sourced with subtype but event declared with base type
		// TODO: Event sourced with (cast) base type and event declared with base type
		// TODO: Two events sourced with (cast) common base types and events declared with two common base types - should work due to explicit casts
		// TODO: Two events sourced with subtypes and events declared with two common base types - weave neither, ambiguous, so build error
		// TODO: One event sourced with two declared events with two common base types - weave neither, ambiguous, so build error
		// TODO: One event sourced (cast) with two declared events with two common base types - should work due to explicit cast
		// TODO: Multiple (heterogenous) event source replacements in the same method
		// TODO: Multiple methods sourcing events need replacements - works now but methods matched on names beginning with 'DoSomething()' !
		// TODO: Search for classes based on [Aggregate] decorations
		// TODO: Protected methods - replacement of event sources
		// TODO: Private methods - replacement of event sources
		// TODO: Internal methods - replacement of event sources
		// TODO: Static methods - replacement of event sources should produce a runtime exception (NotImplementedException)
		// TODO: Protected internal methods - replacement of event sources
		// TODO: Property getters - replacement of event sources
		// TODO: Property setters - replacement of event sources
		// TODO: Check generated method attribute - MethodAttributes.Private
		// TODO: Check generated method attribute - MethodAttributes.PrivateScope
		// TODO: Check generated method attribute - MethodAttributes.Final
		// TODO: Check generated method attribute - MethodAttributes.SpecialName
		// TODO: Check generated method attribute - MethodAttributes.HideBySig
		// TODO: Only detect events that are of type Source.Of<...>
		// TODO: Value Type events...
		// TODO: Interface events...
		// TODO: Methods that access static fields that are NOT Event.Source (ie. don't replace those static field accesses !)
		// TODO: Methods that call methods other than Event.Source.Of<>() (ie. don't replace those other calls !)
		public void Execute()
		{
			if (this.ModuleDefinition == null)
				throw new InvalidOperationException("ModuleDefinition cannot be null.  Is Fody integrated into the build correctly ?");

			var type = this.ModuleDefinition.GetType("Restall.Ichnaea.Fody.AssemblyToProcess.AggregateWithSingleEvent");
			var eventSourcingMethod = this.CreateMethodToRaiseEvent(type);
			type.Methods.Add(eventSourcingMethod);
			foreach (var method in type.Methods.Where(x => x.Name.StartsWith("DoSomething")))
			{
				var il = method.Body.GetILProcessor();
				Instruction of;
				while ((of = method.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Ldsfld)) != null)
					il.Replace(of, il.Create(OpCodes.Ldarg_0));

				Instruction call;
				while ((call = method.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Callvirt)) != null)
					il.Replace(call, il.Create(OpCodes.Call, eventSourcingMethod));
			}
		}

		private MethodDefinition CreateMethodToRaiseEvent(TypeDefinition type)
		{
			var eventDefinition = type.Events[0];
			var eventField = type.Fields.Single(x => x.FullName == eventDefinition.FullName);
			var domainEventType = ((GenericInstanceType) eventField.FieldType).GenericArguments[0];
			var eventDelegate = this.ModuleDefinition.Import(eventField.FieldType.Resolve().Methods.Single(x => x.Name == "Invoke").MakeGenericMethod(domainEventType));

			var noReturnValue = this.ModuleDefinition.TypeSystem.Void;
			var eventSourcingMethod = new MethodDefinition("<Ichnaea>SourceEvent", MethodAttributes.Public, noReturnValue);
			eventSourcingMethod.Parameters.Add(new ParameterDefinition("domainEvent", ParameterAttributes.None, domainEventType));
			eventSourcingMethod.Body.Variables.Add(new VariableDefinition("eventSource", eventField.FieldType));
			eventSourcingMethod.Body.MaxStackSize = 3;
			eventSourcingMethod.Body.InitLocals = true;

			var il = eventSourcingMethod.Body.GetILProcessor();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, eventField);
			il.Emit(OpCodes.Stloc_0);
			il.Emit(OpCodes.Ldloc_0);

			var nullCheckPlaceholder = il.Create(OpCodes.Nop);
			il.Append(nullCheckPlaceholder);
			il.Emit(OpCodes.Ret);

			var startOfCallSequence = il.Create(OpCodes.Ldloc_0);
			il.Append(startOfCallSequence);
			il.Replace(nullCheckPlaceholder, il.Create(OpCodes.Brtrue_S, startOfCallSequence));

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Callvirt, eventDelegate);
			il.Emit(OpCodes.Ret);
			return eventSourcingMethod;
		}

		public ModuleDefinition ModuleDefinition { get; set; }
	}
}
