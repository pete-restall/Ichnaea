using System;
using System.Collections.Generic;

namespace Restall.Ichnaea.NEventStore
{
	public abstract class DisposableContainer: IDisposable
	{
		private readonly List<IDisposable> disposables = new List<IDisposable>();

		protected void AddDisposable(IDisposable disposable)
		{
			this.disposables.Add(disposable);
		}

		public void Dispose()
		{
			this.disposables.ForEach(disposable => disposable.Dispose());
			this.disposables.Clear();
		}
	}
}
