using System;
using System.Threading;
using System.Threading.Tasks;

namespace Schrabber.Models
{
	internal abstract partial class Media : IDisposable
	{
		protected String _cachedLocation = null;

		internal String GetLocation()
		{
				if (this._disposed)
					throw new ObjectDisposedException(nameof(Media));

				if (this._cachedLocation == null)
						throw new InvalidOperationException($"This {nameof(Media)} has not been fetched.");

				return this._cachedLocation;
		}

		protected Media() { }
		protected Media(String location) { this._cachedLocation = location; }

		#region Fetch
		internal virtual Boolean MustFetch => this._cachedLocation == null;
		internal virtual Task FetchTask { get; private protected set; } = Task.CompletedTask;
		internal virtual Task FetchAsync(IProgress<Double> progress = null, CancellationToken token = default) { return Task.CompletedTask; }
		#endregion Fetch

		#region IDisposable
		protected Boolean _disposed { get; private set; } = false;
		public virtual void Dispose() { this._disposed = true; }
		#endregion IDisposableB
	}
}
