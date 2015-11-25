using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Restall.Ichnaea.Fody
{
	public class MethodWeaver
	{
		private readonly MethodDefinition method;
		private readonly EventSourcingMethod[] eventSourcingMethods;

		public MethodWeaver(MethodDefinition method, EventSourcingMethod[] eventSourcingMethods)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			if (eventSourcingMethods == null)
				throw new ArgumentNullException(nameof(eventSourcingMethods));

			this.method = method;
			this.eventSourcingMethods = eventSourcingMethods;
		}

		public void WeaveSourceEventCallsIntoMethod()
		{
			if (this.method.IsStatic)
				return;

			var il = this.method.Body.GetILProcessor();
			Instruction of;
			while ((of = this.method.Body.Instructions.FirstOrDefault(IsLoadFieldSourceEvent)) != null)
				il.Replace(of, il.Create(OpCodes.Ldarg_0));

			Instruction call;
			while ((call = this.method.Body.Instructions.FirstOrDefault(IsCallToSourceEventOf)) != null)
				il.Replace(call, il.Create(OpCodes.Call, this.eventSourcingMethods.Single(x => x.CanReplaceCallToStub((MethodReference) call.Operand)).AsMethodReference()));
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

			var callee = ((MethodReference) instruction.Operand).GetElementMethod();
			return callee.FullName == "System.Void Restall.Ichnaea.Source/EventFluency::Of(!!0)";
		}
	}
}
