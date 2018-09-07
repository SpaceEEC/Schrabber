﻿using Schrabber.Extensions;
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
	internal class InputMediaViewModel : ViewModelBase, IInputMedia
	{
		private BitmapImage _coverImage = null;
		public BitmapImage CoverImage
		{
			get => this._coverImage;
			set => this.SetProperty(ref this._coverImage, value);
		}

		private String _album = String.Empty;
		public String Album
		{
			get => this._album;
			set => this.SetProperty(ref this._album, value);
		}

		private String _title = String.Empty;
		public String Title
		{
			get => this._title;
			set => this.SetProperty(ref this._title, value);
		}

		private String _author = String.Empty;
		public String Author
		{
			get => this._author;
			set => this.SetProperty(ref this._author, value);
		}

		private TimeSpan _duration = TimeSpan.Zero;
		public TimeSpan Duration
		{
			get => this._duration;
			set => this.SetProperty(ref this._duration, value);
		}

		private String _description = String.Empty;
		public String Description
		{
			get => this._description;
			set => this.SetProperty(ref this._description, value);
		}

		private IPart[] _parts = null;
		public IPart[] Parts
		{
			get => this._parts;
			set
			{
				if (this._parts == value) return;

				if (value == null || value.Length == 0)
					this._parts = new IPart[] { new PartViewModel(this) };
				else
					this._parts = value;
				this.OnPropertyChanged();
			}
		}

		private Boolean _disposed = false;
		private MemoryStream _ms = null;

		private readonly String _filePath;
		private readonly String _videoId;

		public InputMediaViewModel(String path, TagLib.File file)
		{
			this._filePath = path;
			this.Author = file.Tag.Performers.FirstOrDefault();
			this.Title = String.IsNullOrWhiteSpace(file.Tag.Title) ? Path.GetFileNameWithoutExtension(file.Name) : file.Tag.Title;

			this.Duration = file.Properties.Duration;
			this.Description = file.Tag.Comment;
			this.Album = file.Tag.Album;
			byte[] coverBytes = (
				file.Tag.Pictures.FirstOrDefault(p => p.Type == TagLib.PictureType.FrontCover)
					?? file.Tag.Pictures.FirstOrDefault()
			)?.Data.Data;

			this._parts = new IPart[] { new PartViewModel(this) };

			if (coverBytes == null || coverBytes.Length == 0) return;
			using (MemoryStream ms = new MemoryStream(coverBytes))
				this.SetImage(ms);
		}

		public InputMediaViewModel(Video video)
		{
			this._videoId = video.Id;

			this.Author = video.Author;
			this.Title = video.Title;
			this.Duration = video.Duration;
			this.Description = video.Description;

			this._parts = new IPart[] { new PartViewModel(this) };

			this.SetImage(new Uri(video.Thumbnails.HighResUrl));
		}

		public void SetImage(Stream stream) => this.CoverImage = ImageHelpers.ResolveBitmapImage(stream: stream);
		public void SetImage(Uri uri) => this.CoverImage = ImageHelpers.ResolveBitmapImage(uri: uri);

		public async Task<MemoryStream> GetMemoryStreamAsync(IProgress<Double> progress = null, CancellationToken token = default(CancellationToken))
		{
			if (this._disposed) throw new InvalidOperationException($"This {nameof(ViewModelBase)} was already disposed.");

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

			throw new InvalidOperationException($"This {nameof(ViewModelBase)} has neither a file nor video to get.");
		}

		#region IDisposable
		public void Dispose()
		{
			if (this._disposed) return;
			this._ms?.Dispose();
			this._disposed = true;
		}
		#endregion IDisposable
	}
}