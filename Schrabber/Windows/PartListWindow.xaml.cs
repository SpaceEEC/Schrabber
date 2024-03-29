﻿using Schrabber.Commands;
using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for PartListWindow.xaml
	/// </summary>
	public partial class PartListWindow : Window
	{
		public static DependencyProperty ListItemsProperty = DependencyProperty.Register(
			nameof(ListItems),
			typeof(ObservableCollection<Part>),
			typeof(PartListWindow),
			new PropertyMetadata(null)
		);

		private readonly Media _media;

		private ObservableCollection<Part> ListItems
		{
			get => (ObservableCollection<Part>)this.GetValue(ListItemsProperty);
			set => this.SetValue(ListItemsProperty, value);
		}

		public Part[] GetParts(Media original)
		{
			return this.ListItems
					.OrderBy(part => part.Start)
					.Select(part => { part.Parent = original; return part; })
					.ToArray();
		}

		public ICommand RemoveItem { get; }

		public PartListWindow(Media media)
		{
			this.InitializeComponent();

			this._media = media;

			this.ListItems = new ObservableCollection<Part>(media.Parts);
			this.RemoveItem = new DelegatedCommand<Part>(m => this.ListItems.Remove(m));
		}

		private void NewPartButton_Click(Object sender, RoutedEventArgs e)
		{
			this.ListItems.Add(new Part(this._media));
		}

		private void ImportPartsButton_Click(Object sender, RoutedEventArgs e)
		{
			PartsGeneratorWindow window = new PartsGeneratorWindow(this._media);
			if (window.ShowDialog() != true) return;

			Part[] parts = window.GetParts();
			if (parts == null) return;

			foreach (Part part in parts)
				this.ListItems.Add(part);
		}

		private void EditAllButton_Click(Object sender, RoutedEventArgs eventArgs)
		{
			EditAllPartsWindow window = new EditAllPartsWindow(this._media);

			HashSet<String> changes = new HashSet<String>();
			window.Part.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
			if (window.ShowDialog() != true) return;

			if (changes.Count == 0) return;

			foreach (Part part in this.ListItems)
			{
				foreach(String change in changes)
				{
					Object value = typeof(Part)
						.GetProperty(change)
						.GetGetMethod()
						.Invoke(window.Part, new Object[0]);

					typeof(Part)
						.GetProperty(change)
						.GetSetMethod()
						.Invoke(
							part,
							new Object[] { value }
						);
				}
			}
		}

		private void RemoveAllPartsButton_Click(Object sender, RoutedEventArgs e) => this.ListItems.Clear();

		private void ConfirmButton_Click(Object sender, RoutedEventArgs e)
		{
			HashSet<String> set = new HashSet<String>();

			String[] groups = this.ListItems
				.GroupBy(part => part.GetFileName())
				.Where(g => g.Count() > 1)
				.Select(grouping => grouping.First().ToString())
				.ToArray();

			if (groups.Length != 0)
			{
				// TODO: ErrorValidation would be far better. But how?
				MessageBox.Show(String.Format(Properties.Resources.PartListWindow_DuplicatesText, String.Join("\n", groups)), Properties.Resources.PartListWindow_DuplicatesTitle);
				e.Handled = true;
			}
			else
			{
				this.DialogResult = true;
			}
		}

		#region ErrorValidation
		// Directly being able to bind to ListBox.Validation.Errors.Count would be nice, but apparently not possible.
		// Or an ICommand for this, but this also works:tm:

		// TODO: Does deleting an erroneous child cause an issue here?

		public static readonly DependencyProperty AllChildrenValidDependencyProperty = DependencyProperty.Register(
			nameof(AllChildrenValid),
			typeof(Boolean),
			typeof(PartListWindow),
			new PropertyMetadata(true)
		);

		private Boolean AllChildrenValid
		{
			get => (Boolean)this.GetValue(AllChildrenValidDependencyProperty);
			set => this.SetValue(AllChildrenValidDependencyProperty, value);
		}

		private Int32 _errors = 0;
		private void PartsListBox_Error(Object sender, ValidationErrorEventArgs e)
		{
			if (e.Action == ValidationErrorEventAction.Added)
				++this._errors;
			else
				--this._errors;

			if (this._errors == 0)
				this.AllChildrenValid = true;
			else if (this._errors == 1)
				this.AllChildrenValid = false;
		}
		#endregion ErrorValidation
	}
}
