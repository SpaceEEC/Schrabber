using System;
using System.Threading;
using System.Threading.Tasks;

namespace Schrabber.Models
{
	public abstract partial class Media : IDisposable
	{
		protected String _cachedLocation = null;

		public String GetLocation()
		{
				if (this._disposed)
					throw new ObjectDisposedException(nameof(Media));

				if (this._cachedLocation == null)
						throw new InvalidOperationException($"This {nameof(Media)} has not been fetched.");

				return this._cachedLocation;
		}

		protected Media() { }
		protected Media(String location) { this._cachedLocation = location; }

		public abstract Media GetCopy();

		#region Fetch
		public virtual Boolean MustFetch => this._cachedLocation == null;
		public virtual Task FetchTask { get; private protected set; } = Task.CompletedTask;
		public virtual Task FetchAsync(IProgress<Double> progress = null, CancellationToken token = default) { return Task.CompletedTask; }
		#endregion Fetch

		#region IDisposable
		protected Boolean _disposed { get; private set; } = false;
		public virtual void Dispose() { this._disposed = true; }
		#endregion IDisposableB
	}
}
