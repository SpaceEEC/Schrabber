using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Schrabber.Interfaces
{
	/// <summary>
	/// Interface for media input.
	/// </summary>
	public interface IInputMedia : INotifyPropertyChanged
	{
		/// <summary>
		/// BitmapImage of the CoverImage of this IInputMedia.
		/// </summary>
		BitmapImage CoverImage { get; set; }

		/// <summary>
		/// Initial album of this IInputMedia.
		/// </summary>
		String Album { get; set; }

		/// <summary>
		/// Initial title of this IInputMedia.
		/// </summary>
		String Title { get; set; }

		/// <summary>
		/// Initial author of this IInputMedia.
		/// </summary>
		String Author { get; set; }

		/// <summary>
		/// Full duration of this IInputMedia.
		/// </summary>
		TimeSpan Duration { get; set; }

		/// <summary>
		/// Description or other additional data of this IInputMedia.
		/// </summary>
		String Description { get; set; }

		/// <summary>
		/// An array of IParts this IInputMedia should be splitted into.
		/// </summary>
		IPart[] Parts { get; set; }

		/// <summary>
		/// Sets a new image from a stream.
		/// </summary>
		/// <param name="stream">Stream to read from</param>
		void SetImage(Stream stream);


		/// <summary>
		/// Sets a new image from an uri
		/// </summary>
		/// <param name="uri">Uri source</param>
		void SetImage(Uri uri);

		/// <summary>
		/// Gets the MemoryStream this IInputMedia represents.
		/// </summary>
		/// <returns>Task with a result of MemoryStream</returns>
		Task<MemoryStream> GetMemoryStreamAsync(IProgress<Double> progress = null, CancellationToken token = default(CancellationToken));
	}
}
