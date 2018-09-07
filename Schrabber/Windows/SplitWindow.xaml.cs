using Schrabber.Interfaces;
using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Schrabber.Windows
{
	/// <summary>
	/// Window for the user to separate an IInputMedia into IParts.
	/// </summary>
	public partial class SplitWindow : Window
	{
		public static DependencyProperty ListItemsProperty = DependencyProperty.Register(
			nameof(ListItems),
			typeof(ObservableCollection<IPart>),
			typeof(SplitWindow),
			new PropertyMetadata(null)
		);

		/// <summary>
		/// IEnumerable of IParts the user separated the IInputmedia into.
		/// </summary>
		public IEnumerable<IPart> Parts
		{
			get => this.ListItems.OrderBy(p => p.Start);
		}

		private readonly IInputMedia _media;

		private ObservableCollection<IPart> ListItems
		{
			get => (ObservableCollection<IPart>)this.GetValue(ListItemsProperty);
			set => this.SetValue(ListItemsProperty, value);
		}

		public SplitWindow(IInputMedia input)
		{
			this.InitializeComponent();

			this._media = input;
			this.ListItems = new ObservableCollection<IPart>(input.Parts);
		}

		private void ConfirmButton_Click(Object sender, RoutedEventArgs e)
		{
			IPart prev = null;
			foreach (IPart row in this.ListItems)
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

		private void ImportPartsButton_Click(Object sender, RoutedEventArgs e)
		{
			SplitAssistWindow window = new SplitAssistWindow(this._media);
			if (window.ShowDialog() != true) return;

			foreach (IPart part in window.Parts)
				this.ListItems.Add(part);
		}

		private void NewPartButton_Click(Object sender, RoutedEventArgs e)
		{
			PartWindow window = new PartWindow()
			{
				Part = new PartViewModel(this._media)
			};
			if (window.ShowDialog() != true) return;

			this.ListItems.Add(window.Part);
		}

		private void RemovePartButton_Click(Object sender, RoutedEventArgs e) => this.ListItems.Remove((IPart)this.PartsListBox.SelectedItem);
		private void PartsListBox_MouseDoubleClick(Object sender, MouseButtonEventArgs e) => this._doEdit((IPart)this.PartsListBox.SelectedItem);
		private void EditPartButton_Click(Object sender, RoutedEventArgs e) => this._doEdit((IPart)this.PartsListBox.SelectedItem);
		private void _doEdit(IPart original)
		{
			if (original == null) return;
			IPart copy = original.GetCopy();
			PartWindow window = new PartWindow()
			{
				Part = copy
			};
			if (window.ShowDialog() != true) return;

			this.ListItems.Remove(original);
			this.ListItems.Add(copy);
		}

		private void RemoveAllPartsButton_Click(Object sender, RoutedEventArgs e) => this.ListItems.Clear();
	}
}
