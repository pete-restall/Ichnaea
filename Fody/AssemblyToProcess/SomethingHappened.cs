using System;
using System.Diagnostics.CodeAnalysis;

namespace Restall.Ichnaea.Fody.AssemblyToProcess
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = CodeAnalysisJustification.JsonSerialisation)]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = CodeAnalysisJustification.JsonSerialisation)]
	public class SomethingHappened
	{
		public SomethingHappened(Guid token)
		{
			this.Token = token;
		}

		public Guid Token { get; private set; }
	}
}
