using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using NullGuard;

namespace Restall.Ichnaea
{
	[Serializable]
	[NullGuard(ValidationFlags.None)]
	public class AggregateRootIdPropertyNotFoundException: AggregateRootIdNotFoundException
	{
		public AggregateRootIdPropertyNotFoundException()
		{
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
		public AggregateRootIdPropertyNotFoundException(string message): base(message)
		{
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.ExceptionPola)]
		public AggregateRootIdPropertyNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected AggregateRootIdPropertyNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.PropertyName = info.GetOrDefault<string>(nameof(this.PropertyName), null);
		}

		public AggregateRootIdPropertyNotFoundException(Type aggregateRootType, Type aggregateRootIdType, string propertyName)
			: base(
				"The expected Aggregate Root ID Property was not found; " +
				"aggregateRootType=" + (aggregateRootType ?? typeof(object)).FullName +
				", aggregateRootIdType=" + (aggregateRootIdType ?? typeof(object)).FullName +
				", propertyName=" + propertyName)
		{
			if (aggregateRootType == null)
				throw new ArgumentNullException(nameof(aggregateRootType));

			if (aggregateRootIdType == null)
				throw new ArgumentNullException(nameof(aggregateRootIdType));

			if (propertyName == null)
				throw new ArgumentNullException(nameof(propertyName));

			this.AggregateRootType = aggregateRootType;
			this.AggregateRootIdType = aggregateRootIdType;
			this.PropertyName = propertyName;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddOrDefault(nameof(this.PropertyName), PropertyName, _ => null);
		}

		public string PropertyName { get; }
	}
}
