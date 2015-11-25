using System;
using System.Linq;
using Mono.Cecil;

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

		public void WeaveSourceEventMethodIntoAggregateRoot()
		{
			if (!this.IsTypeAggregateRoot() || !this.AreEventsDeclaredCorrectly())
				return;

			var eventSourcingMethods = this.type.Events.Select(x => new EventSourcingMethod(x)).ToArray();
			foreach (var eventSourcingMethod in eventSourcingMethods)
				eventSourcingMethod.WeaveSourceEventMethodIntoAggregateRoot();

			foreach (var method in this.type.Methods)
				new MethodWeaver(method, eventSourcingMethods).WeaveSourceEventCallsIntoMethod();
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
	}
}
