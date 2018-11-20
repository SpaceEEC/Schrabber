using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	public abstract partial class Media : Base
	{
		protected BitmapImage _coverImage = null;
		public override BitmapImage CoverImage
		{
			get => this._coverImage;
			set => this.SetProperty(ref this._coverImage, value);
		}

		protected String _album = String.Empty;
		public String Album
		{
			get => this._album;
			set => this.SetProperty(ref this._album, value);
		}

		protected String _title = String.Empty;
		public override String Title
		{
			get => this._title;
			set => this.SetProperty(ref this._title, value);
		}

		protected String _author = String.Empty;
		public override String Author
		{
			get => this._author;
			set => this.SetProperty(ref this._author, value);
		}

		protected TimeSpan _duration = TimeSpan.Zero;
		public TimeSpan Duration
		{
			get => this._duration;
			set => this.SetProperty(ref this._duration, value);
		}

		protected String _description = String.Empty;
		public String Description
		{
			get => this._description;
			set => this.SetProperty(ref this._description, value);
		}

		protected Part[] _parts = null;
		public Part[] Parts
		{
			get => (this._parts?.Length ?? 0) == 0
				? this._parts = new[] { new Part(this) }
				: this._parts;
			set => this.SetProperty(ref this._parts, value);
		}
	}
}
