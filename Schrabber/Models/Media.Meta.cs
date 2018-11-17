using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	internal abstract partial class Media : Base
	{
		private BitmapImage _coverImage = null;
		public override BitmapImage CoverImage
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
		public override String Title
		{
			get => this._title;
			set => this.SetProperty(ref this._title, value);
		}

		private String _author = String.Empty;
		public override String Author
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

		private Part[] _parts = null;
		public Part[] Parts
		{
			get => (this._parts?.Length ?? 0) == 0
				? this._parts = new[] { new Part(this) }
				: this._parts;
			set => this.SetProperty(ref this._parts, value);
		}
	}
}
