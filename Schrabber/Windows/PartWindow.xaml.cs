using Microsoft.Win32;
using Schrabber.Helpers;
using Schrabber.Interfaces;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for PartWindow.xaml
	/// </summary>
	public partial class PartWindow : Window
	{
		private Int32 _errors = 0;

		public IPart Part { get; }
		public PartWindow(IPart part)
		{
			this.Part = part;
			this.InitializeComponent();

			this.RemoveCover_MenuItem.IsEnabled = part.HasCoverImage;
		}

		private void ValidationError(object sender, ValidationErrorEventArgs e)
		{
			if (e.Action == ValidationErrorEventAction.Added)
				++this._errors;
			else
				--this._errors;
		}
		private void CanConfirm(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this._errors == 0;
			e.Handled = true;
		}

		private void ExecuteConfirm(object sender, ExecutedRoutedEventArgs e)
		{
			this.DialogResult = true;
			e.Handled = true;
		}

		private void SetCover_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = OpenFileDialogFactory.GetImageFileDialog();
			ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

			if (ofd.ShowDialog() != true) return;

			using (FileStream fs = new FileInfo(ofd.FileName).OpenRead())
				this.Part.CoverImage = ImageHelpers.ResolveBitmapImage(stream: fs);

			this.RemoveCover_MenuItem.IsEnabled = true;
			this.CoverImage.Source = this.Part.CoverImage;
		}

		private void RemoveCover_Click(object sender, RoutedEventArgs e)
		{
			this.RemoveCover_MenuItem.IsEnabled = false;
			this.Part.CoverImage = null;
			this.CoverImage.Source = this.Part.Parent.CoverImage;
		}
	}
}
