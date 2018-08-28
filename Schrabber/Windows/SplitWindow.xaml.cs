using Schrabber.Interfaces;
using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Schrabber.Windows
{
	/// <summary>
	/// Window for the user to separate an IInputMedia into IParts.
	/// </summary>
	public partial class SplitWindow : Window
	{
		/// <summary>
		/// IEnumerable of IParts the user separated the IInputmedia into.
		/// </summary>
		public IEnumerable<IPart> Parts
		{
			get => this._listItems.OrderBy(p => p.Start);
		}

		private readonly IInputMedia _media;
		private readonly ObservableCollection<IPart> _listItems;

		public SplitWindow(IInputMedia input)
		{
			this.InitializeComponent();

			this._media = input;
			this._listItems = new ObservableCollection<IPart>(input.Parts);

			this.PartsListBox.ItemsSource = this._listItems;
		}

		private void ConfirmButton_Click(object sender, RoutedEventArgs e)
		{
			IPart prev = null;
			foreach (IPart row in this._listItems)
			{
				if (prev != null)
				{
					if (prev.Start == row.Start)
					{
						MessageBox.Show($"Identical timestamp found for: \n \"{prev.Title}\"\n\"{row.Title}\"");

						e.Handled = true;
						return;
					}
					prev.Stop = row.Start;
				}

				prev = row;
			}

			this.DialogResult = true;
			this.Close();
		}

		private void ImportPartsButton_Click(object sender, RoutedEventArgs e)
		{
			// TODO: :that:
			MessageBox.Show("Not implemented yet.");
		}

		private void NewPartButton_Click(object sender, RoutedEventArgs e)
		{
			PartWindow window = new PartWindow(new Part(this._media));
			if (window.ShowDialog() != true) return;

			this._listItems.Add(window.Part);
		}

		private void RemovePartButton_Click(object sender, RoutedEventArgs e) => this._listItems.Remove((IPart)this.PartsListBox.SelectedItem);
		private void PartsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) => this._doEdit((IPart)this.PartsListBox.SelectedItem);
		private void EditPartButton_Click(object sender, RoutedEventArgs e) => this._doEdit((IPart)this.PartsListBox.SelectedItem);
		private void _doEdit(IPart original)
		{
			if (original == null) return;
			IPart copy = original.GetCopy();
			PartWindow window = new PartWindow(copy);
			if (window.ShowDialog() != true) return;

			this._listItems.Remove(original);
			this._listItems.Add(copy);
		}
	}
}
