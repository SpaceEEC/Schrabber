using Microsoft.Win32;
using Schrabber.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Schrabber.Controls
{
	/// <summary>
	/// Interaction logic for PartControl.xaml
	/// </summary>
	public partial class PartControl : UserControl
	{
		#region Cover
		private void SetCover_Click(Object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog()
			{
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
				Multiselect = false
			};
			if (ofd.ShowDialog() != true) return;

			try
			{
				using (Stream stream = ofd.OpenFile())
					((Media)((FrameworkElement)sender).DataContext).SetBitmapImage(stream: stream);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void RemoveCover_Click(Object sender, RoutedEventArgs e)
			=> ((Media)((FrameworkElement)sender).DataContext).CoverImage = null;
		#endregion Cover

		#region Validation
		public Boolean IsPartValid => this._errors == 0;

		public PartControl() => this.InitializeComponent();

		private Int32 _errors = 0;
		private void ValidationError(Object sender, ValidationErrorEventArgs e)
		{
			if (e.Action == ValidationErrorEventAction.Added)
				++this._errors;
			else
				--this._errors;
		}
		#endregion Validation

		#region Button Properties
		public static readonly DependencyProperty ButtonNameDependencyProperty = DependencyProperty.Register(
			nameof(ButtonName),
			typeof(String),
			typeof(PartControl),
			new PropertyMetadata("Delete")
		);

		public String ButtonName
		{
			get => (String)this.GetValue(ButtonNameDependencyProperty);
			set => this.SetValue(ButtonNameDependencyProperty, value);
		}

		public static readonly DependencyProperty ButtonDefaultDependencyProperty = DependencyProperty.Register(
			nameof(ButtonDefault),
			typeof(Boolean),
			typeof(PartControl),
			new PropertyMetadata(true)
		);

		public Boolean ButtonDefault
		{
			get => (Boolean)this.GetValue(ButtonDefaultDependencyProperty);
			set => this.SetValue(ButtonDefaultDependencyProperty, value);
		}

		public static readonly DependencyProperty ButtonCommandDependencyProperty =
			DependencyProperty.Register(
			nameof(ButtonCommand),
			typeof(ICommand),
			typeof(PartControl),
			new UIPropertyMetadata(null)
		);

		public ICommand ButtonCommand
		{
			get => (ICommand)this.GetValue(ButtonCommandDependencyProperty);
			set => this.SetValue(ButtonCommandDependencyProperty, value);
		}
		#endregion Button Properties
	}
}
