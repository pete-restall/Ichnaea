using System;
using System.Linq;
using Mono.Cecil;

namespace Restall.Ichnaea.Fody
{
	internal static class MethodReferenceExtensions
	{
		public static MethodReference MakeGenericMethod(this MethodReference method, params TypeReference[] arguments)
		{
			var reference = new MethodReference(method.Name, method.ReturnType, method.DeclaringType.CreateGenericInstanceType(arguments))
				{
					HasThis = method.HasThis,
					ExplicitThis = method.ExplicitThis,
					CallingConvention = method.CallingConvention
				};

			foreach (var parameter in method.Parameters.Select(x => new ParameterDefinition(x.ParameterType)))
				reference.Parameters.Add(parameter);

			foreach (var genericParameter in method.GenericParameters.Select(x => new GenericParameter(x.Name, reference)))
				reference.GenericParameters.Add(genericParameter);

			return reference;
		}

		private static TypeReference CreateGenericInstanceType(this TypeReference genericType, params TypeReference[] arguments)
		{
			if (genericType.GenericParameters.Count != arguments.Length)
			{
				throw new ArgumentException(
					"Expected " + genericType.GenericParameters.Count + " type arguments for type " + genericType.FullName,
					nameof(arguments));
			}

			var instanceType = new GenericInstanceType(genericType);
			foreach (var argument in arguments)
				instanceType.GenericArguments.Add(argument);

			return instanceType;
		}
	}
}
