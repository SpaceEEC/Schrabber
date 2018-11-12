using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	internal abstract partial class Media : ViewModelBase
	{
		private BitmapImage _coverImage = null;
		internal BitmapImage CoverImage
		{
			get => this._coverImage;
			set => this.SetProperty(ref this._coverImage, value);
		}

		private String _album = String.Empty;
		internal String Album
		{
			get => this._album;
			set => this.SetProperty(ref this._album, value);
		}

		private String _title = String.Empty;
		internal String Title
		{
			get => this._title;
			set => this.SetProperty(ref this._title, value);
		}

		private String _author = String.Empty;
		internal String Author
		{
			get => this._author;
			set => this.SetProperty(ref this._author, value);
		}

		private TimeSpan _duration = TimeSpan.Zero;
		internal TimeSpan Duration
		{
			get => this._duration;
			set => this.SetProperty(ref this._duration, value);
		}

		private String _description = String.Empty;
		internal String Description
		{
			get => this._description;
			set => this.SetProperty(ref this._description, value);
		}

		private Object[] _parts = null;
		internal Object[] Parts
		{
			get => this._parts;
			set => this.SetProperty(ref this._parts, value);
		}

		// TODO: Move me somewhere better. Again a static class?
		protected BitmapImage ResolveBitmapImage(Stream stream = null, Uri uri = null)
		{
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.StreamSource = stream;
			image.UriSource = uri;
			image.EndInit();
			if (image.CanFreeze) image.Freeze();

			return image;
		}
	}
}
