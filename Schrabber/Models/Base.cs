using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	public abstract class Base : ViewModelBase
	{
		protected BitmapImage _coverImage = null;
		public virtual BitmapImage CoverImage
		{
			get => this._coverImage;
			set
			{
				if (this.SetProperty(ref this._coverImage, value))
					this.OnPropertyChanged(nameof(this.HasCoverImage));
			}
		}
		public Boolean HasCoverImage => this._coverImage != null;

		protected String _album = String.Empty;
		public virtual String Album
		{ 
			get => this._album;
			set => this.SetProperty(ref this._album, value);
		}

		protected String _author = String.Empty;
		public virtual String Author
		{
			get => this._author;
			set => this.SetProperty(ref this._author, value);
		}

		protected String _title = String.Empty;
		public virtual String Title
		{
			get => this._title;
			set => this.SetProperty(ref this._title, value);
		}

		public void SetBitmapImage(Stream stream = null, Uri uri = null)
		{
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.StreamSource = stream;
			image.UriSource = uri;
			image.EndInit();
			if (image.CanFreeze) image.Freeze();

			this.CoverImage = image;
		}

		public override String ToString()
		{
			if (String.IsNullOrWhiteSpace(this.Author))
				return this.Title;

			return $"{this.Author} - {this.Title}";
		}
	}
}
