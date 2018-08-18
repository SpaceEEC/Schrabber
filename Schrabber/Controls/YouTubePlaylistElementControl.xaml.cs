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
			InitializeComponent();

			Media = media;

			if (media.CoverImage != null) ThumbnailImage.Source = media.CoverImage;
			InformationTextBlock.Text = String.IsNullOrWhiteSpace(media.Author) ? $"{media.Author} - " : "";
			InformationTextBlock.Text += $"{media.Title}\n\n{media.Description}";
		}

		private void ToggleKeepButton_Click(object sender, RoutedEventArgs e)
		{
			Keep = !Keep;
			ToggleKeepButton.Content = $"        {(Keep ? "Keep" : "Ignore")}\n\n(Click to toggle)";
		}
	}
}
