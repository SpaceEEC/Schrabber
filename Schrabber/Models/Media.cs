using System;
using System.Threading;
using System.Threading.Tasks;

namespace Schrabber.Models
{
	internal abstract partial class Media : IDisposable
	{
		protected String _cachedLocation = null;

		internal virtual Boolean MustFetch => this._cachedLocation == null;

		internal String Location
		{
			get
			{
				if (this._disposed)
					throw new ObjectDisposedException(nameof(Media));

				if (this._cachedLocation == null)
						throw new InvalidOperationException($"This {nameof(Media)} has not been fetched.");

				return this._cachedLocation;
			}
		}

		protected Media() { }
		protected Media(String location) { this._cachedLocation = location; }

		internal virtual Task FetchAsync(IProgress<Double> progress = null, CancellationToken token = default) { return Task.CompletedTask; }

		protected Boolean _disposed { get; private set; } = false;
		public virtual void Dispose() { this._disposed = true; }
	}
}
