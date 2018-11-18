using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode.Models;
using YoutubeExplode.Converter;
using System.Threading;
using YoutubeExplode;
using Schrabber.Workers;
using System.Linq;

namespace Schrabber.Models
{
	internal class YoutubeMedia : Media
	{
		private static readonly YoutubeClient _client = new YoutubeClient();
		private static YoutubeConverter Converter => new YoutubeConverter(YoutubeMedia._client, FFmpeg.FindExecutablePath());

		private readonly String _videoId;

		internal YoutubeMedia() : base()
		{
			this.FetchTask = this.Tsc.Task;
		}
		internal YoutubeMedia(Video video) : this()
		{
			this._videoId = video.Id;

			this.Author = video.Author;
			this.Title = video.Title;
			this.Duration = video.Duration;
			this.Description = video.Description;
			this.SetBitmapImage(uri: new Uri(video.Thumbnails.HighResUrl));
		}
		internal override Media GetCopy() => new YoutubeMedia(this);

		private YoutubeMedia(YoutubeMedia orig) : this()
		{
			if (this._disposed) throw new ObjectDisposedException(nameof(LocalMedia));

			this._album = orig._album;
			this._author = orig._author;
			this._cachedLocation = orig._cachedLocation;
			this._coverImage = orig._coverImage;
			this._description = orig._description;
			this._duration = orig._duration;
			this._parts = orig._parts?.Select(part => new Part(part)).ToArray();
			this._title = orig._title;
		}

		#region Fetch
		private readonly TaskCompletionSource<Object> Tsc = new TaskCompletionSource<Object>();
		internal async override Task FetchAsync(
			IProgress<Double> progress = null,
			CancellationToken token = default
		) {
			if (this._disposed) throw new ObjectDisposedException(nameof(Media));
			if (this._cachedLocation != null) return;
			if (this.FetchTask.IsCompleted) return;

			String path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");

			try
			{
				await YoutubeMedia.Converter.DownloadVideoAsync(
					this._videoId,
					path,
					"mp3",
					progress,
					token
				);
			}
			catch (Exception exception)
			{
				try { File.Delete(path); }
				catch { }

				this.Tsc.SetException(exception);

				throw;
			}

			this._cachedLocation = path;
			this.Tsc.SetResult(null);
		}
		#endregion Fetch

		#region IDisposable
		public override void Dispose()
		{
			if (this._disposed) return;

			if (this._cachedLocation != null)
			{
				try { File.Delete(this._cachedLocation); }
				catch { }
			}

			base.Dispose();
		}
		#endregion IDisposable
	}
}
