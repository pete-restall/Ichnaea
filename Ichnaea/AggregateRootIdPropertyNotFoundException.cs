using System;
using System.Runtime.Serialization;
using NullGuard;

namespace Restall.Ichnaea
{
	[Serializable]
	[NullGuard(ValidationFlags.None)]
	public class AggregateRootIdPropertyNotFoundException: Exception
	{
		public AggregateRootIdPropertyNotFoundException()
		{
		}

		public AggregateRootIdPropertyNotFoundException(string message): base(message)
		{
		}

		public AggregateRootIdPropertyNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected AggregateRootIdPropertyNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public AggregateRootIdPropertyNotFoundException(Type aggregateRootType, Type aggregateRootIdType, string propertyName)
			: base(
				"The expected Aggregate Root ID Property was not found; " +
				"aggregateRootType=" + (aggregateRootType ?? typeof(object)).FullName +
				", aggregateRootIdType=" + (aggregateRootIdType ?? typeof(object)).FullName +
				", propertyName=" + propertyName)
		{
			if (aggregateRootType == null)
				throw new ArgumentNullException("aggregateRootType");

			if (aggregateRootIdType == null)
				throw new ArgumentNullException("aggregateRootIdType");

			if (propertyName == null)
				throw new ArgumentNullException("propertyName");

			this.AggregateRootType = aggregateRootType;
			this.AggregateRootIdType = aggregateRootIdType;
			this.PropertyName = propertyName;
		}

		public Type AggregateRootType { get; private set; }

		public Type AggregateRootIdType { get; private set; }

		public string PropertyName { get; private set; }
	}
}
