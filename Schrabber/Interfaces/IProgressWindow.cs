using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schrabber.Interfaces
{
	/// <summary>
	/// Interface a ProgressWindow has to implement.
	/// </summary>
	public interface IProgressWindow
	{
		/// <summary>
		/// The total amount IInputMedias to process.
		/// </summary>
		Int32 TotalMediaCount { get; set; }

		/// <summary>
		/// The progress of the current step.
		/// </summary>
		Double Progress { get;  set; }

		/// <summary>
		/// The current step.
		/// </summary>
		String Step { get;  set; }

		/// <summary>
		/// The currently processed media.
		/// </summary>
		IInputMedia CurrentMedia { get;  set; }

		/// <summary>
		/// Skips to the next part.
		/// </summary>
		void NextPart();
	}
}
