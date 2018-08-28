using Schrabber.Interfaces;
using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	internal class Part : IPart
	{
		public IInputMedia Parent { get; set; }

		private BitmapImage _coverImage = null;
		public BitmapImage CoverImage { get => _coverImage ?? Parent.CoverImage; set => _coverImage = value; }
		public Boolean HasCoverImage { get => _coverImage != null;  }

		private String _album = null;
		public String Album { get => _album ?? Parent.Album; set => _album = value; }

		private String _author = null;
		public String Author { get => _author ?? Parent.Author; set => _author = value; }

		public TimeSpan Start { get; set; } = TimeSpan.FromSeconds(0);

		private TimeSpan? _stop = null;
		public TimeSpan Stop { get => _stop ?? Parent.Duration; set => _stop = value; } 

		private String _title = null;

		public String Title { get => _title ?? Parent.Title; set => _title = value; }

		internal Part(IInputMedia parent) => Parent = parent;

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
	}
}
