using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Schrabber.Interfaces
{
	/// <summary>
	/// Interface for the remote removal of UIElements from a parent.
	/// </summary>
	public interface IRemoveChildElement
	{
		/// <summary>
		/// Removes a children from the parent.
		/// </summary>
		/// <param name="child">The children to remove.</param>
		void RemoveChildElement(UIElement child);
	}
}
