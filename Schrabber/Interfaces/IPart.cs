using System;

namespace Schrabber.Interfaces
{
	/// <summary>
	/// Interface for parts of audio tracks.
	/// </summary>
	public interface IPart
	{
		/// <summary>
		/// The absolute position where this IPart starts.
		/// </summary>
		TimeSpan Start { get; set; }

		/// <summary>
		/// The absolute position where this IPart stops.
		/// 
		/// Note:
		///   Can be null if to the end.
		/// </summary>
		TimeSpan? Stop { get; set; }

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
	}
}
