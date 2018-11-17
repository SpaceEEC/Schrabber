using System;
using System.Linq;
using System.IO;


namespace Schrabber.Models
{
	internal class LocalMedia : Media
	{
		internal LocalMedia(String path, TagLib.File file) : base(path)
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
	}
}
