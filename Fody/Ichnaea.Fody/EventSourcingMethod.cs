using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Restall.Ichnaea.Fody
{
	public class EventSourcingMethod
	{
		private const MethodAttributes SourceEventMethodAttributes =
			MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

		private readonly MethodDefinition eventSourcingMethod;
		private readonly EventDefinition eventDefinition;
		private readonly TypeDefinition domainEventType;

		public EventSourcingMethod(EventDefinition eventDefinition)
		{
			if (eventDefinition == null)
				throw new ArgumentNullException(nameof(eventDefinition));

			this.eventSourcingMethod = CreateSourceEventMethodFor(eventDefinition);
			this.eventDefinition = eventDefinition;
			this.domainEventType = ((GenericInstanceType) eventDefinition.EventType).GenericArguments[0].Resolve();
		}

		private static MethodDefinition CreateSourceEventMethodFor(EventDefinition eventDefinition)
		{
			var eventField = eventDefinition.DeclaringType.Fields.Single(x => x.FullName == eventDefinition.FullName);
			var domainEventType = ((GenericInstanceType) eventField.FieldType).GenericArguments[0];
			var invokeMethod = eventField.FieldType.Resolve().Methods.Single(x => x.Name == "Invoke").MakeGenericMethod(domainEventType);
			var eventDelegate = eventDefinition.DeclaringType.Module.ImportReference(invokeMethod);

			var noReturnValue = eventDefinition.DeclaringType.Module.TypeSystem.Void;
			var eventSourcingMethod = new MethodDefinition("<Ichnaea>SourceEvent", SourceEventMethodAttributes, noReturnValue);
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

		public void WeaveSourceEventMethodIntoAggregateRoot()
		{
			this.eventDefinition.DeclaringType.Methods.Add(this.eventSourcingMethod);
		}

		public bool CanReplaceCallToStub(MethodReference stubMethod)
		{
			if (stubMethod == null)
				throw new ArgumentNullException(nameof(stubMethod));

			// TODO: Probably some sort of ranking - highest is same type, then lower rankings for base types (based on depth ?)
			return this.domainEventType.IsAssignableFrom(((GenericInstanceMethod) stubMethod).GenericArguments[0].Resolve());
		}

		public MethodReference AsMethodReference()
		{
			return this.eventSourcingMethod;
		}
	}
}
