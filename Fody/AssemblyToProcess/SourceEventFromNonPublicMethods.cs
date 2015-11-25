using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[AggregateRoot]
	[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local", Justification = CodeAnalysisJustification.StubForTesting)]
	[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global", Justification = CodeAnalysisJustification.StubForTesting)]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.StubForTesting)]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = CodeAnalysisJustification.StubForTesting)]
	public class SourceEventFromNonPublicMethods
	{
		public void SourceEventFromPrivateMethod(Guid token)
		{
			this.PrivateMethod(token);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void PrivateMethod(Guid token)
		{
			Source.Event.Of(new SomethingHappened(token));
		}

		public void SourceEventFromProtectedMethod(Guid token)
		{
			this.ProtectedMethod(token);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected void ProtectedMethod(Guid token)
		{
			Source.Event.Of(new SomethingHappened(token));
		}

		public void SourceEventFromProtectedInternalMethod(Guid token)
		{
			this.ProtectedInternalMethod(token);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected internal void ProtectedInternalMethod(Guid token)
		{
			Source.Event.Of(new SomethingHappened(token));
		}

		public void SourceEventFromInternalMethod(Guid token)
		{
			this.InternalMethod(token);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void InternalMethod(Guid token)
		{
			Source.Event.Of(new SomethingHappened(token));
		}

		[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global", Justification = CodeAnalysisJustification.IchnaeaSubscribes)]
		public event Source.Of<SomethingHappened> EventSource;
	}
}
