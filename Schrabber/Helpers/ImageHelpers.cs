using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Schrabber.Helpers
{
	internal static class ImageHelpers
	{
		public static BitmapImage ResolveBitmapImage(Stream stream = null, Uri uri = null)
		{
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.StreamSource = stream;
			image.UriSource = uri;
			image.EndInit();
			if (image.CanFreeze) image.Freeze();

			return image;
		}
	}
}
