using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	public class Part : Base
	{
		public Media Parent { get; set; }

		public override BitmapImage CoverImage
		{
			get => base.CoverImage ?? this.Parent.CoverImage;
			set => base.CoverImage = value;
		}

		public override String Album
		{
			get => base.Album ?? this.Parent.Album;
			set => base.Album = value;
		}

		public override String Title
		{
			get => base.Title ?? this.Parent.Title;
			set => base.Title = value;
		}

		public override String Author
		{
			get => base.Author ?? this.Parent.Author;
			set => base.Author = value;
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
			this._album = null;
			this._author = null;
			this._title = null;
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

		public String GetFileName()
		{
			String fileName = this.ToString();

			return String.Join(
				"_",
				fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)
			// TODO: Configurable extension
			).TrimEnd('.') + ".mp3";
		}
	}
}
