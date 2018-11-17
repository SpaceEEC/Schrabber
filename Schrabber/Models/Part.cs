using System;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	internal class Part : Base
	{
		public readonly Media Parent;

		private BitmapImage _coverImage = null;
		public override BitmapImage CoverImage
		{
			get => this._coverImage ?? this.Parent.CoverImage;
			set
			{
				if (this.SetProperty(ref this._coverImage, value))
					this.OnPropertyChanged(nameof(this.HasCoverImage));
			}
		}
		internal Boolean HasCoverImage => this._coverImage != null;

		private String _album = null;
		public String Album
		{
			get => this._album ?? this.Parent.Album;
			set => this.SetProperty(ref this._album, value);
		}

		private String _title = null;
		public override String Title
		{
			get => this._title ?? this.Parent.Title;
			set => this.SetProperty(ref this._title, value);
		}

		private String _author = null;
		public override String Author
		{
			get => this._author ?? this.Parent.Author;
			set => this.SetProperty(ref this._author, value);
		}

		private TimeSpan? _start = null;
		public TimeSpan? Start
		{
			get => this._start;
			set => this.SetProperty(ref this._start, value);
		}

		private TimeSpan? _stop = null;
		public TimeSpan? Stop
		{
			get => this._stop;
			set => this.SetProperty(ref this._stop, value);
		}

		internal Part(Media parent)
		{
			this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
		}
	}
}
