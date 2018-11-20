using System;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	public class Part : Base
	{
		public Media Parent { get; }

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
		public Boolean HasCoverImage => this._coverImage != null;

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

		public Part(Media parent)
		{
			this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
		}

		public Part(Part part) : this(part.Parent)
		{
			this._album = part._album;
			this._author = part._author;
			this._coverImage = part._coverImage;
			this._start = part._start;
			this._stop = part._stop;
			this._title = part._title;
		}
	}
}
