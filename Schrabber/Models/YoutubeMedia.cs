using Schrabber.Workers;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;

namespace Schrabber.Models
{
	public class YoutubeMedia : Media
	{
		private readonly VideoId _videoId;

		public YoutubeMedia() : base()
		{
			this.FetchTask = this.Tcs.Task;
		}
		public YoutubeMedia(IVideo video) : this()
		{
			this._videoId = video.Id;

			this.Author = video.Author.Title;
			this.Title = video.Title;
			this.Duration = video.Duration.Value;

			var thumbnailUrl = video.Thumbnails.OrderByDescending(t => t.Resolution.Area).First().Url;
			this.SetBitmapImage(uri: new Uri(thumbnailUrl));
		}
		public YoutubeMedia(Video video) : this((IVideo)video)
		{
			this.Description = video.Description;
		}


		public override Media GetCopy() => new YoutubeMedia(this);

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
		private readonly TaskCompletionSource<Object> Tcs = new TaskCompletionSource<Object>();
		public async override Task FetchAsync(
			IProgress<Double> progress = null,
			CancellationToken token = default
		) {
			if (this._disposed) throw new ObjectDisposedException(nameof(Media));
			if (this._cachedLocation != null) return;
			if (this.FetchTask.IsCompleted) return;

			String path = Cache.GetTempCacheFilename();

			try
			{
				await Youtube.Client.Videos.DownloadAsync(this._videoId, path, progress, token);
			}
			catch (Exception exception)
			{
				try { File.Delete(path); }
				catch { }

				this.Tcs.SetException(exception);

				throw exception;
			}

			this._cachedLocation = path;
			this.Tcs.SetResult(null);
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
