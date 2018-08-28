using Microsoft.Win32;
using Schrabber.Helpers;
using Schrabber.Interfaces;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for PartWindow.xaml
	/// </summary>
	public partial class PartWindow : Window
	{
		public IPart Part { get; }
		public PartWindow(IPart part)
		{
			Part = part;
			InitializeComponent();

			this.RemoveCover_MenuItem.IsEnabled = part.HasCoverImage;
		}

		private void DefaultButton_Click(object sender, RoutedEventArgs e)
		{
			if (TimeSpan.TryParse(StartTextBox.Text, out TimeSpan start))
			{ 
				if (start < TimeSpan.FromSeconds(0))
				{
					MessageBox.Show("\"Start\" may not be negative.");

					return;
				}

			}
			else
			{
				MessageBox.Show("Parsing of \"Start\" failed, make sure it's a valid time.");

				return;
			}
			if (TimeSpan.TryParse(StopTextBox.Text, out TimeSpan stop))
			{
				if (this.Part.Parent.Duration < stop)
				{
					MessageBox.Show("\"Stop\" may not be greater than the original duration of the media.");
					return;
				}

				DialogResult = true;
			}
			else
			{
				MessageBox.Show("Parsing of \"Stop\" failed, make sure it's a valid time.");
			}
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
