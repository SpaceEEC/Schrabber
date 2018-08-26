using Schrabber.Extensions;
using Schrabber.Helpers;
using Schrabber.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using YoutubeExplode.Models;

namespace Schrabber.Models
{
	internal class InputMedia : IInputMedia
	{
		public BitmapImage CoverImage { get; set; } = null;
		public string Title { get; set; } = "";
		public string Author { get; set; } = "";
		public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0);
		public string Description { get; set; } = "";

		private IPart[] _parts = null;
		public IPart[] Parts
		{
			get => this._parts;
			set
			{
				if (value == null || value.Length == 0)
					this._parts = new IPart[] { this._getDefault() };
				else
					this._parts = value;
			}
		}

		private Boolean _disposed = false;
		private MemoryStream _ms = null;

		private readonly String _filePath;
		private readonly String _videoId;

		public InputMedia(String path, TagLib.File file)
		{
			this._filePath = path;
			this.Author = file.Tag.Performers.FirstOrDefault();
			this.Title = String.IsNullOrWhiteSpace(file.Tag.Title) ? Path.GetFileNameWithoutExtension(file.Name) : file.Tag.Title;

			byte[] bytes = (
				file.Tag.Pictures.FirstOrDefault(p => p.Type == TagLib.PictureType.FrontCover)
					?? file.Tag.Pictures.FirstOrDefault()
			)?.Data.Data;
			this.Duration = file.Properties.Duration;
			this.Description = file.Tag.Comment;

			this._parts = new IPart[] { this._getDefault() };

			if (bytes == null || bytes.Length == 0) return;
			using (MemoryStream ms = new MemoryStream(bytes))
				this.SetImage(ms);
		}

		public InputMedia(Video video)
		{
			this._videoId = video.Id;

			this.Author = video.Author;
			this.Title = video.Title;
			this.Duration = video.Duration;
			this.Description = video.Description;

			this._parts = new IPart[] { this._getDefault() };

			this.SetImage(new Uri(video.Thumbnails.HighResUrl));
		}

		public void SetImage(Stream stream) => this._setImage(stream: stream);
		public void SetImage(Uri uri) => this._setImage(uri: uri);

		private void _setImage(Stream stream = null, Uri uri = null)
		{
			this.CoverImage = new BitmapImage();
			this.CoverImage.BeginInit();
			this.CoverImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			this.CoverImage.CacheOption = BitmapCacheOption.OnLoad;
			this.CoverImage.StreamSource = stream;
			this.CoverImage.UriSource = uri;
			this.CoverImage.EndInit();
			if (this.CoverImage.CanFreeze) this.CoverImage.Freeze();
		}

		public void Dispose()
		{
			if (this._disposed) return;
			this._ms?.Dispose();
			this._disposed = true;
		}

		public async Task<MemoryStream> GetMemoryStreamAsync(IProgress<Double> progress = null, CancellationToken token = default(CancellationToken))
		{
			if (this._disposed) throw new InvalidOperationException("This IInputMedia was already disposed.");

			if (this._ms != null) return this._ms;

			if (this._filePath != null)
			{
				this._ms = new MemoryStream();
				await new FileStream(this._filePath, FileMode.Open).CopyToAsync(this._ms, token: token);
				return this._ms;
			}

			if (this._videoId != null)
			{
				return this._ms = await YouTubeClient
					.DownloadYouTubeVideoMp3MemoryStreamAsync(this._videoId, TimeSpan.FromSeconds(0), progress, token)
					.ConfigureAwait(false);
			}

			throw new InvalidOperationException("This IInputMedia has neither a file nor video to get.");
		}

		private IPart _getDefault()
		{
			return new Part()
			{
				Author = Author,
				Start = TimeSpan.FromSeconds(0),
				Title = Title,
			};
		}
	}
}
