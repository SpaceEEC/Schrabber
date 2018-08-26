using Schrabber.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Schrabber.Controls
{
	/// <summary>
	/// Interaction logic for YouTubePlaylistElementControl.xaml
	/// </summary>
	public partial class YouTubePlaylistElementControl : UserControl
	{
		/// <summary>
		/// The IInputMedia this YouTubePlayListElementControl was instantiated with.
		/// </summary>
		public IInputMedia Media { get; }

		/// <summary>
		/// Whether the user wants to keep the IInputElement associated with this YouTubePlaylistElementCtontrol.
		/// </summary>
		public Boolean Keep { get; private set; } = true;


		/// <summary>
		/// Instantiates a new YouTubePlaylistElementControl.
		/// </summary>
		/// <param name="media">The IInputMedia to associate this InputElementControl with.</param>
		public YouTubePlaylistElementControl(IInputMedia media)
		{
			this.InitializeComponent();

			this.Media = media;

			if (media.CoverImage != null) this.ThumbnailImage.Source = media.CoverImage;
			this.InformationTextBlock.Text = String.IsNullOrWhiteSpace(media.Author) ? $"{media.Author} - " : "";
			this.InformationTextBlock.Text += $"{media.Title}\n\n{media.Description}";
		}

		private void ToggleKeepButton_Click(object sender, RoutedEventArgs e)
		{
			this.Keep = !this.Keep;
			this.ToggleKeepButton.Content = $"        {(this.Keep ? "Keep" : "Ignore")}\n\n(Click to toggle)";
		}
	}
}
