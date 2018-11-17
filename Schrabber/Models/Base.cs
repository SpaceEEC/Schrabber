using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	internal abstract class Base : ViewModelBase
	{
		public abstract String Author { get; set; }
		public abstract String Title { get; set; }
		
		public abstract BitmapImage CoverImage { get; set; }

		internal void SetBitmapImage(Stream stream = null, Uri uri = null)
		{
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.StreamSource = stream;
			image.UriSource = uri;
			image.EndInit();
			if (image.CanFreeze) image.Freeze();

			this.CoverImage = image;
		}

		public override String ToString()
		{
			if (String.IsNullOrWhiteSpace(this.Author))
				return this.Title;

			return $"{this.Author} - {this.Title}";
		}
	}
}
