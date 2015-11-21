using System;
using System.Collections.Generic;

namespace Restall.Ichnaea
{
	public abstract class DisposableContainer: IDisposable
	{
		private readonly List<IDisposable> disposables = new List<IDisposable>();

		~DisposableContainer()
		{
			this.Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			this.disposables.ForEach(disposable => disposable.Dispose());
			this.disposables.Clear();
			GC.SuppressFinalize(this);
		}

		protected void AddDisposable(IDisposable disposable)
		{
			this.disposables.Add(disposable);
		}

		protected void RemoveDisposable(IDisposable disposable)
		{
			this.disposables.Remove(disposable);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}
	}
}
