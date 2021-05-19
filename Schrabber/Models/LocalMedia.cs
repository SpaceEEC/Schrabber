using System;
using System.IO;
using System.Linq;


namespace Schrabber.Models
{
	public class LocalMedia : Media
	{
		public LocalMedia(String path, TagLib.File file) : base(path)
		{
			this.Author = file.Tag.Performers.FirstOrDefault();
			this.Title = String.IsNullOrWhiteSpace(file.Tag.Title)
				? Path.GetFileNameWithoutExtension(file.Name)
				: file.Tag.Title;
			this.Duration = file.Properties.Duration;
			this.Description = file.Tag.Comment;
			this.Album = file.Tag.Album;

			TagLib.IPicture cover = file.Tag.Pictures.FirstOrDefault(p => p.Type == TagLib.PictureType.FrontCover) ?? file.Tag.Pictures.FirstOrDefault();

			if (cover?.Data?.Data != null)
			{
				using (MemoryStream ms = new MemoryStream(cover.Data.Data))
					this.SetBitmapImage(stream: ms);
			}
		}

		public override Media GetCopy() => new LocalMedia(this);

		private LocalMedia(LocalMedia orig) : base(orig._cachedLocation)
		{
			if (this._disposed) throw new ObjectDisposedException(nameof(LocalMedia));

			this._album = orig._album;
			this._author = orig._author;
			this._coverImage = orig._coverImage;
			this._description = orig._description;
			this._duration = orig._duration;
			this._parts = orig._parts?.Select(part => new Part(part)).ToArray();
			this._title = orig._title;
		}
	}
}
