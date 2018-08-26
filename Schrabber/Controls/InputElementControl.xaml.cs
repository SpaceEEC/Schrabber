using Microsoft.Win32;
using Schrabber.Helpers;
using Schrabber.Interfaces;
using Schrabber.Windows;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Schrabber.Controls
{
	/// <summary>
	/// Interaction logic for InputElementControl.xaml
	/// </summary>
	public partial class InputElementControl : UserControl, IDisposable
	{
		/// <summary>
		/// The IInputMedia this InputElementControl was instantiated with.
		/// </summary>
		public IInputMedia Media { get; }

		private IRemoveChildElement _parent;
		private Boolean _disposed = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent">Parent this InputElementControl will be added to.</param>
		/// <param name="media">The IInputMedia to associate this InputElementControl with.</param>
		public InputElementControl(IInputMedia media, IRemoveChildElement parent)
		{
			InitializeComponent();
			_parent = parent;
			Media = media;

			if (media.CoverImage != null) ThumbnailImage.Source = media.CoverImage;
			InformationTextBlock.Text = String.IsNullOrWhiteSpace(media.Author) ? $"{media.Author} - " : "";
			InformationTextBlock.Text += $"{media.Title}\n\n{media.Description}";
			_updateButton();
		}

		private void _updateButton()
		{
			SplitButton.Content = new TextBlock()
			{
				Text = $"Split\n\nParts: {Media.Parts.Length}",
				TextAlignment = TextAlignment.Center,
			};
		}

		private void SplitButton_Click(object sender, RoutedEventArgs e)
		{
			SplitWindow window = new SplitWindow(Media);
			if (window.ShowDialog() != true) return;
			IPart[] parts = window.Parts.ToArray();
			for (Int32 i = 0; i > parts.Length; ++i)
			{
				if (i + 1 == parts.Length)
				{
					parts[i].Stop = null;
					break;
				}

				parts[i].Stop = parts[i + 1].Start;
			}

			Media.Parts = parts;

			_updateButton();
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e) => _parent.RemoveChildElement(this);

		public void Dispose()
		{
			if (_disposed) return;
			Media.Dispose();
			_disposed = true;
		}

		private void SetCover_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog ofd = OpenFileDialogFactory.GetImageFileDialog();
			ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

			if (ofd.ShowDialog() != true) return;

			using (FileStream fs = new FileInfo(ofd.FileName).OpenRead())
				Media.SetImage(fs);

			ThumbnailImage.Source = Media.CoverImage;
		}

		private void RemoveCover_Click(object sender, RoutedEventArgs e)
		{
			Media.CoverImage = null;
			ThumbnailImage.Source = new BitmapImage(new Uri("/Schrabber;component/resources/no_cover.jpg", UriKind.Relative));
		}
	}
}
