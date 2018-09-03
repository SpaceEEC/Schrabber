using Schrabber.Interfaces;
using System;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	internal class PartViewModel : ViewModelBase, IPart
	{
		private IInputMedia _parent;
		public IInputMedia Parent
		{
			get => this._parent;
			set => this.SetProperty(ref this._parent, value);
		}

		private BitmapImage _coverImage = null;
		public BitmapImage CoverImage
		{
			get => this._coverImage ?? this.Parent.CoverImage;
			set
			{
				if (this.SetProperty(ref this._coverImage, value))
					this.OnPropertyChanged(nameof(this.HasCoverImage));
			}
		}
		public Boolean HasCoverImage { get => this._coverImage != null; }

		private String _album = null;
		public String Album
		{
			get => this._album ?? this.Parent.Album;
			set => this.SetProperty(ref this._album, value);
		}

		private String _author = null;
		public String Author
		{
			get => this._author ?? this.Parent.Author;
			set => this.SetProperty(ref this._author, value);
		}

		private TimeSpan _start = TimeSpan.Zero;
		public TimeSpan Start
		{
			get => this._start;
			set => this.SetProperty(ref this._start, value);
		}

		private TimeSpan? _stop = null;
		public TimeSpan Stop
		{
			get => this._stop ?? this.Parent.Duration;
			set => this.SetProperty(ref this._stop, value);
		}

		private String _title = null;

		public String Title
		{
			get => this._title ?? this.Parent.Title;
			set => this.SetProperty(ref this._title, value);
		}

		internal PartViewModel(IInputMedia parent) => this.Parent = parent;

		public IPart GetCopy()
		{
			return new PartViewModel(this.Parent)
			{
				_coverImage = this._coverImage,
				_album = this._album,
				_author = this._author,
				Start = this.Start,
				_stop = this._stop,
				_title = this._title
			};
		}
	}
}
