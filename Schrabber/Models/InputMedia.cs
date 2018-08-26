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
			get => _parts;
			set
			{
				if (value == null || value.Length==0)
					_parts = new IPart[] { _getDefault() };
				else
					_parts = value;
			}
		}

		private Boolean _disposed = false;
		private MemoryStream _ms = null;

		private readonly String _filePath;
		private readonly String _videoId;

		public InputMedia(String path, TagLib.File file)
		{
			_filePath = path;
			Author = file.Tag.Performers.FirstOrDefault();
			Title = String.IsNullOrWhiteSpace(file.Tag.Title) ? Path.GetFileNameWithoutExtension(file.Name) : file.Tag.Title;

			byte[] bytes = (
				file.Tag.Pictures.FirstOrDefault(p => p.Type == TagLib.PictureType.FrontCover)
					?? file.Tag.Pictures.FirstOrDefault()
			)?.Data.Data;
			Duration = file.Properties.Duration;
			Description = file.Tag.Comment;

			_parts = new IPart[] { _getDefault() };

			if (bytes == null || bytes.Length == 0) return;
			using (MemoryStream ms = new MemoryStream(bytes))
				SetImage(ms);
		}

		public InputMedia(Video video)
		{
			_videoId = video.Id;

			Author = video.Author;
			Title = video.Title;
			Duration = video.Duration;
			Description = video.Description;

			_parts = new IPart[] { _getDefault() };

			SetImage(new Uri(video.Thumbnails.HighResUrl));
		}

		public void SetImage(Stream stream) => _setImage(stream: stream);
		public void SetImage(Uri uri) => _setImage(uri: uri);

		private void _setImage(Stream stream = null, Uri uri = null)
		{
			CoverImage = new BitmapImage();
			CoverImage.BeginInit();
			CoverImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			CoverImage.CacheOption = BitmapCacheOption.OnLoad;
			CoverImage.StreamSource = stream;
			CoverImage.UriSource = uri;
			CoverImage.EndInit();
			if (CoverImage.CanFreeze) CoverImage.Freeze();
		}

		public void Dispose()
		{
			if (_disposed) return;
			_ms?.Dispose();
			_disposed = true;
		}

		public async Task<MemoryStream> GetMemoryStreamAsync(IProgress<Double> progress = null, CancellationToken token = default(CancellationToken))
		{
			if (_disposed) throw new InvalidOperationException("This IInputMedia was already disposed.");

			if (_ms != null) return _ms;

			if (_filePath != null)
			{
				_ms = new MemoryStream();
				await new FileStream(_filePath, FileMode.Open).CopyToAsync(_ms, token: token);
				return _ms;
			}

			if (_videoId != null)
			{
				return _ms = await YouTubeClient
					.DownloadYouTubeVideoMp3MemoryStreamAsync(_videoId, TimeSpan.FromSeconds(0), progress, token)
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
