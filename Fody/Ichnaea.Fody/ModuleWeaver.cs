using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Restall.Ichnaea.Fody
{
	public class ModuleWeaver
	{
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
		// TODO: Protected methods - replacement of event sources
		// TODO: Private methods - replacement of event sources
		// TODO: Internal methods - replacement of event sources
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
		// TODO: Async methods...
		// TODO: Source.Event() inside a closure
		// TODO: Private events
		// TODO: Protected events
		// TODO: Protected internal events
		// TODO: Internal events
		// TODO: Source.Event() CALLED IN METHOD IN (PUBLIC) NESTED CLASS
		// TODO: Source.Event() CALLED IN METHOD IN (PRIVATE) NESTED CLASS
		// TODO: Source.Event() CALLED IN METHOD IN (PROTECTED) NESTED CLASS
		// TODO: Source.Event() CALLED IN METHOD IN (PROTECTED INTERNAL) NESTED CLASS
		// TODO: Source.Event() CALLED IN METHOD IN (INTERNAL) NESTED CLASS
		public void Execute()
		{
			if (this.ModuleDefinition == null)
				throw new InvalidOperationException("ModuleDefinition cannot be null.  Is Fody integrated into the build correctly ?");

			foreach (var type in this.ModuleDefinition.GetTypes())
				this.WeaveRaiseEventIntoAggregateRoot(type);
		}

		private void WeaveRaiseEventIntoAggregateRoot(TypeDefinition type)
		{
			if (type.CustomAttributes.All(x => x.AttributeType.FullName != "Restall.Ichnaea.AggregateRootAttribute"))
				return;

			var eventSourcingMethod = this.CreateMethodToRaiseEvent(type);
			if (eventSourcingMethod == null)
				return;

			type.Methods.Add(eventSourcingMethod);
			foreach (var method in type.Methods.Where(x => !x.IsStatic))
			{
				var il = method.Body.GetILProcessor();
				Instruction of;
				while ((of = method.Body.Instructions.FirstOrDefault(IsLoadFieldSourceEvent)) != null)
					il.Replace(of, il.Create(OpCodes.Ldarg_0));

				Instruction call;
				while ((call = method.Body.Instructions.FirstOrDefault(IsCallToSourceEventOf)) != null)
					il.Replace(call, il.Create(OpCodes.Call, eventSourcingMethod));
			}
		}

		private MethodDefinition CreateMethodToRaiseEvent(TypeDefinition type)
		{
			var eventDefinition = type.Events.FirstOrDefault();
			if (eventDefinition == null)
				return null;

			var eventField = type.Fields.Single(x => x.FullName == eventDefinition.FullName);
			var domainEventType = ((GenericInstanceType) eventField.FieldType).GenericArguments[0];
			var eventDelegate = this.ModuleDefinition.ImportReference(eventField.FieldType.Resolve().Methods.Single(x => x.Name == "Invoke").MakeGenericMethod(domainEventType));

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

		private static bool IsLoadFieldSourceEvent(Instruction instruction)
		{
			if (instruction.OpCode != OpCodes.Ldsfld)
				return false;

			var field = (FieldReference) instruction.Operand;
			return field.FullName == "Restall.Ichnaea.Source/EventFluency Restall.Ichnaea.Source::Event";
		}

		private static bool IsCallToSourceEventOf(Instruction instruction)
		{
			if (instruction.OpCode != OpCodes.Callvirt)
				return false;

			var method = ((MethodReference) instruction.Operand).GetElementMethod();
			return method.FullName == "System.Void Restall.Ichnaea.Source/EventFluency::Of(!!0)";
		}

		public ModuleDefinition ModuleDefinition { private get; set; }
	}
}
