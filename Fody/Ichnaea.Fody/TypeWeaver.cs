using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Restall.Ichnaea.Fody
{
	public class TypeWeaver
	{
		private const MethodAttributes SourceEventMethodAttributes =
			MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

		private readonly TypeDefinition type;

        public TypeWeaver(TypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

	        this.type = type;
		}

		public void WeaveRaiseEventIntoAggregateRoot()
		{
			if (!this.IsTypeAggregateRoot() || !this.AreEventsDeclaredCorrectly())
				return;

			var eventRaisingMethod = this.AddMethodToTypeForRaisingNativeEvent();
			foreach (var method in this.type.Methods)
				new MethodWeaver(method, eventRaisingMethod).WeaveSourceEventIntoMethod();
		}

		private bool IsTypeAggregateRoot()
		{
			return this.type.CustomAttributes.Any(x => x.AttributeType.FullName == "Restall.Ichnaea.AggregateRootAttribute");
		}

		private bool AreEventsDeclaredCorrectly()
		{
			return
				this.type.Events.Any() &&
				this.type.Events.All(x => x.EventType.FullName.StartsWith("Restall.Ichnaea.Source/Of`1<")) &&
				this.type.Events.Select(x => x.EventType.FullName).Distinct().Count() == this.type.Events.Count;
		}

		private MethodDefinition AddMethodToTypeForRaisingNativeEvent()
		{
			var eventDefinition = this.type.Events[0];
			var eventField = this.type.Fields.Single(x => x.FullName == eventDefinition.FullName);
			var domainEventType = ((GenericInstanceType) eventField.FieldType).GenericArguments[0];
			var eventDelegate = this.type.Module.ImportReference(eventField.FieldType.Resolve().Methods.Single(x => x.Name == "Invoke").MakeGenericMethod(domainEventType));

			var noReturnValue = this.type.Module.TypeSystem.Void;
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

			this.type.Methods.Add(eventSourcingMethod);
			return eventSourcingMethod;
		}
	}
}
