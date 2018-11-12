using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Schrabber.Models
{
	// https://stackoverflow.com/a/36151255
	/// <summary>
	/// ViewModelBase classed implementing essential methods and interfaces for view models.
	/// </summary>
	internal abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Fires the PropertyChanged EventHandler.
		/// </summary>
		/// <param name="propertyName">The name of the property which changed</param>
		protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Updates a property, fires the PropertyChanged EventHandler if the value changed.
		/// </summary>
		/// <typeparam name="T">The type of the property</typeparam>
		/// <param name="storage">Reference to the property</param>
		/// <param name="value">The new value</param>
		/// <param name="propertyName">The name of the property</param>
		/// <returns>Whether the property updated</returns>
		protected virtual Boolean SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

			storage = value;
			this.OnPropertyChanged(propertyName);

			return true;
		}
	}
}
