using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode.Models;
using YoutubeExplode.Converter;
using System.Threading;

namespace Schrabber.Models
{
	internal class YoutubeMedia : Media
	{
		private readonly static YoutubeConverter _converter = new YoutubeConverter();

		private readonly String _videoId;

		internal YoutubeMedia(Video video) : base()
		{
			this._videoId = video.Id;

			this.Author = video.Author;
			this.Title = video.Title;
			this.Duration = video.Duration;
			this.Description = this.Description;
			this.CoverImage = this.ResolveBitmapImage(uri: new Uri(video.Thumbnails.HighResUrl));
		}

		internal async override Task FetchAsync(
			IProgress<Double> progress = null,
			CancellationToken token = default
		) {
			if (this._disposed) throw new ObjectDisposedException(nameof(Media));
			if (this._cachedLocation != null) return;

			String path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");

			try
			{
				await YoutubeMedia._converter.DownloadVideoAsync(
					this._videoId,
					path,
					"mp3",
					progress,
					token
				);
			}
			catch
			{
				try { File.Delete(path); }
				catch { }

				throw;
			}

			this._cachedLocation = path;
		}

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
	}
}
