using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Restall.Ichnaea.Fody
{
	public class MethodWeaver
	{
		private readonly MethodDefinition method;
		private readonly MethodDefinition eventSourcingMethod;

		public MethodWeaver(MethodDefinition method, MethodDefinition eventSourcingMethod)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			if (eventSourcingMethod == null)
				throw new ArgumentNullException(nameof(eventSourcingMethod));

			this.method = method;
			this.eventSourcingMethod = eventSourcingMethod;
		}

		public void WeaveSourceEventIntoMethod()
		{
			if (this.method.IsStatic)
				return;

			var il = this.method.Body.GetILProcessor();
			Instruction of;
			while ((of = this.method.Body.Instructions.FirstOrDefault(IsLoadFieldSourceEvent)) != null)
				il.Replace(of, il.Create(OpCodes.Ldarg_0));

			Instruction call;
			while ((call = this.method.Body.Instructions.FirstOrDefault(IsCallToSourceEventOf)) != null)
				il.Replace(call, il.Create(OpCodes.Call, this.eventSourcingMethod));
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
