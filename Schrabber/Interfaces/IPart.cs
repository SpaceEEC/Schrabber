using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Schrabber.Interfaces
{
	/// <summary>
	/// Interface for parts of audio tracks.
	/// </summary>
	public interface IPart : INotifyPropertyChanged
	{
		/// <summary>
		/// The parent of this IPart.
		/// </summary>
		IInputMedia Parent { get; set; }

		/// <summary>
		/// The cover of this IPart, if not set defaults to the associated IInputMedia's image.
		/// </summary>
		BitmapImage CoverImage { get; set; }

		/// <summary>
		/// Whether this IPart has a custom CoverImage set.
		/// </summary>
		Boolean HasCoverImage { get; }

		/// <summary>
		/// The absolute position where this IPart starts.
		/// </summary>
		TimeSpan Start { get; set; }

		/// <summary>
		/// The absolute position where this IPart stops.
		/// </summary>
		TimeSpan Stop { get; set; }

		/// <summary>
		/// The title of this IPart.
		/// </summary>
		String Title { get; set; }

		/// <summary>
		/// The author of this IPart.
		/// </summary>
		String Author { get; set; }

		/// <summary>
		/// The album this IPart is part of.
		/// </summary>
		String Album { get; set; }

		/// <summary>
		/// Gets an exact copy of this IPart.
		/// </summary>
		/// <returns></returns>
		IPart GetCopy();
	}
}
