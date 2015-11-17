﻿using System;
using System.Runtime.Serialization;
using NullGuard;

namespace Restall.Ichnaea
{
	[Serializable]
	[NullGuard(ValidationFlags.None)]
	public class AggregateRootIdNotFoundException: Exception
	{
		public AggregateRootIdNotFoundException()
		{
		}

		public AggregateRootIdNotFoundException(string message): base(message)
		{
		}

		public AggregateRootIdNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected AggregateRootIdNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public AggregateRootIdNotFoundException(Type aggregateRootType, Type aggregateRootIdType)
			: base(
				"The expected Aggregate Root ID Property was not found; " +
				"aggregateRootType=" + (aggregateRootType ?? typeof(object)).FullName +
				", aggregateRootIdType=" + (aggregateRootIdType ?? typeof(object)).FullName)
		{
			if (aggregateRootType == null)
				throw new ArgumentNullException(nameof(aggregateRootType));

			if (aggregateRootIdType == null)
				throw new ArgumentNullException(nameof(aggregateRootIdType));

			this.AggregateRootType = aggregateRootType;
			this.AggregateRootIdType = aggregateRootIdType;
		}

		public Type AggregateRootType { get; protected set; }

		public Type AggregateRootIdType { get; protected set; }
	}
}
