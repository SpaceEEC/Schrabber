using Schrabber.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	internal class Part : IPart
	{
		private IInputMedia _parent;
		public IInputMedia Parent
		{
			get => this._parent;
			set
			{
				if (this._parent == value) return;

				this._parent = value;
				this.NotifyPropertyChanged();
			}
		}

		private BitmapImage _coverImage = null;
		public BitmapImage CoverImage
		{
			get => this._coverImage ?? this.Parent.CoverImage;
			set
			{
				if (this._coverImage == value) return;

				Boolean willHasCoverImageChange = this._coverImage == null || value == null;

				this._coverImage = value;
				this.NotifyPropertyChanged();
				if (willHasCoverImageChange)
					this.NotifyPropertyChanged(nameof(this.HasCoverImage));
			}
		}
		public Boolean HasCoverImage { get => this._coverImage != null; }

		private String _album = null;
		public String Album
		{
			get => this._album ?? this.Parent.Album;
			set
			{
				if (this._album == value) return;

				this._album = value;
				this.NotifyPropertyChanged();
			}
		}

		private String _author = null;
		public String Author
		{
			get => this._author ?? this.Parent.Author;
			set
			{
				if (this._author == value) return;

				this._author = value;
				this.NotifyPropertyChanged();
			}
		}

		private TimeSpan _start = TimeSpan.Zero;
		public TimeSpan Start
		{
			get => this._start;
			set
			{
				if (this._start == value) return;

				this._start = value;
				this.NotifyPropertyChanged();
			}
		}

		private TimeSpan? _stop = null;
		public TimeSpan Stop
		{
			get => this._stop ?? this.Parent.Duration;
			set
			{
				if (this._stop == value) return;

				this._stop = value;
				this.NotifyPropertyChanged();
			}
		}

		private String _title = null;

		public String Title
		{
			get => this._title ?? this.Parent.Title;
			set
			{
				if (this._title == value) return;

				this._title = value;
				this.NotifyPropertyChanged();
			}
		}

		internal Part(IInputMedia parent) => this.Parent = parent;

		public IPart GetCopy()
		{
			return new Part(this.Parent)
			{
				_coverImage = this._coverImage,
				_album = this._album,
				_author = this._author,
				Start = this.Start,
				_stop = this._stop,
				_title = this._title
			};
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (this.PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion INotifyPropertyChanged

	}
}
