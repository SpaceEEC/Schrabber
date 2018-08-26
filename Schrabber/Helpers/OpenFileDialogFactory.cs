using Microsoft.Win32;
using System;

namespace Schrabber.Helpers
{
	/// <summary>
	/// Class instantiating Microsoft.Win32.OpenFileDialogs with filters
	/// </summary>
	internal static class OpenFileDialogFactory
	{
		public const String AllFiles = "All files (*.*)|*.*";

		#region images
		// https://stackoverflow.com/q/17736160
		public const String Png = "PNG Portable Network Graphics (*.png)|*.png";
		public const String Jpg = "JPEG File Interchange Format (*.jpg *.jpeg *jfif)|*.jpg;*.jpeg;*.jfif";
		public const String Bmp = "BMP Windows Bitmap (*.bmp)|*.bmp";
		public const String Tif = "TIF Tagged Imaged File Format (*.tif *.tiff)|*.tif;*.tiff";
		public const String Gif = "GIF Graphics Interchange Format (*.gif)|*.gif";
		public const String AllImages = "Image Files|*.png; *.jpg; *.jpeg; *.jfif; *.bmp;*.tif; *.tiff; *.gif";
		#endregion images

		#region videos/music
		public const String Webm = "WEBM (*.webm)|*.webm";
		public const String Mpeg = "MPEG (*.mpg *.mpeg *.mpg1 *.mp2 *.mp3 *.m1v *.m1a *m2a *.mpa *.mpv *.mp4 *.m4a *.m4p *.m4b *.m4r *.m4v|" +
			"*.mpg; *.mpeg; *.mpg1; *.mp2; *.mp3; *.m1v; *.m1a; *m2a; *.mpa; *.mpv; *.mp4; *.m4a; *.m4p; *.m4b; *.m4r; *.m4v";
		public const String Ogg = "OGG (*.ogg *.ogv *.oga *.ogx *.ogm *.spx *.opus)|" +
			"*.ogg; *.ogv; *.oga; *.ogx; *.ogm; *.spx; *.opus";
		public const String Aac = "AAC (*.aac)|*.aac";
		public const String Flv = "FLV Flash video format (*.flv)|*.flv";
		public const String Avi = "AVI AVI format (*.avi)|*.avi";
		public const String AllVideoMusic = "Video and Music Files|" +
			"*.webm; *.mpg; *.mpeg; *.mpg1; *.mp2; *.mp3; *.m1v; *.m1a; *m2a; *.mpa; *.mpv; *.mp4; *.m4a; *.m4p; *.m4b; *.m4r; *.m4v; " +
			"*.mpg; *.mpeg; *.mpg1; *.mp2; *.mp3; *.m1v; *.m1a; *m2a; *.mpa; *.mpv; *.mp4; *.m4a; *.m4p; *.m4b; *.m4r; *.m4v; " +
			"*.ogg; *.ogv; *.oga; *.ogx; *.ogm; *.spx; *.opus; *.acc; *.flv; *.avi";
		#endregion videos/music

		/// <summary>
		/// All video and music files, including "AllFiles" filter.
		/// </summary>
		public static String[] VideoMusicFilters { get => new String[] { Webm, Mpeg, Ogg, Aac, Flv, Avi, AllVideoMusic, AllFiles }; }

		/// <summary>
		/// All image files, including "AllFiles" filter.
		/// </summary>
		public static String[] ImageFilters { get => new String[] { Png, Jpg, Bmp, Tif, Gif, AllImages, AllFiles }; }

		/// <summary>
		/// Gets an OpenFileDialog with image filters set.
		/// </summary>
		/// <returns>The created OpenFileDialog</returns>
		public static OpenFileDialog GetImageFileDialog() => new OpenFileDialog() { Filter = String.Join("|", ImageFilters), FilterIndex = ImageFilters.Length - 1 };

		/// <summary>
		/// Gets an OpenFileDialog with video and music filters set.
		/// </summary>
		/// <returns>The created OpenFileDialog</returns>
		public static OpenFileDialog GetVideoAndMusicFileDialog() => new OpenFileDialog() { Filter = String.Join("|", VideoMusicFilters), FilterIndex = VideoMusicFilters.Length - 1 };
	}
}
