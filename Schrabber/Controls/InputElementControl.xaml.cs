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
			this.InitializeComponent();
			this._parent = parent;
			this.Media = media;

			if (media.CoverImage != null) this.ThumbnailImage.Source = media.CoverImage;
			this.InformationTextBlock.Text = String.IsNullOrWhiteSpace(media.Author) ? $"{media.Author} - " : "";
			this.InformationTextBlock.Text += $"{media.Title}\n\n{media.Description}";
			this._updateButton();
		}

		private void _updateButton()
		{
			this.SplitButton.Content = new TextBlock()
			{
				Text = $"Split\n\nParts: {this.Media.Parts.Length}",
				TextAlignment = TextAlignment.Center,
			};
		}

		private void SplitButton_Click(object sender, RoutedEventArgs e)
		{
			SplitWindow window = new SplitWindow(this.Media);
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

			this.Media.Parts = parts;

			this._updateButton();
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e) => this._parent.RemoveChildElement(this);

		public void Dispose()
		{
			if (this._disposed) return;
			this.Media.Dispose();
			this._disposed = true;
		}

		private void SetCover_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = OpenFileDialogFactory.GetImageFileDialog();
			ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

			if (ofd.ShowDialog() != true) return;

			using (FileStream fs = new FileInfo(ofd.FileName).OpenRead())
				this.Media.SetImage(fs);

			this.ThumbnailImage.Source = this.Media.CoverImage;
		}

		private void RemoveCover_Click(object sender, RoutedEventArgs e)
		{
			this.Media.CoverImage = null;
			this.ThumbnailImage.Source = new BitmapImage(new Uri("/Schrabber;component/resources/no_cover.jpg", UriKind.Relative));
		}
	}
}
