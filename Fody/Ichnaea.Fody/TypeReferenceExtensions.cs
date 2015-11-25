using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Restall.Ichnaea.Fody
{
	public static class TypeReferenceExtensions
	{
		public static bool IsAssignableFrom(this TypeReference baseType, TypeDefinition type)
		{
			var queue = new Queue<TypeDefinition>();
			queue.Enqueue(type);
			while (queue.Any())
			{
				var current = queue.Dequeue();
				if (baseType.FullName == current.FullName)
					return true;

				if (current.BaseType != null)
					queue.Enqueue(current.BaseType.Resolve());

				foreach (var @interface in current.Interfaces)
					queue.Enqueue(@interface.Resolve());
			}

			return false;
		}
	}
}
