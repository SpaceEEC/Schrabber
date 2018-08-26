using Schrabber.Controls;
using Schrabber.Helpers;
using Schrabber.Interfaces;
using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YoutubeExplode;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Models;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for YouTubePlaylistWindow.xaml
	/// </summary>
	public partial class YouTubePlaylistWindow : Window
	{

		/// <summary>
		/// IEnumerable of, from the user, selected IInputMedias.
		/// </summary>
		public IEnumerable<IInputMedia> SelectedMedias
		{
			get =>
				this.PlaylistElementStackPanel
					.Children
					.OfType<YouTubePlaylistElementControl>()
					.Where(c => c.Keep)
					.Select(c => c.Media);
		}
		public YouTubePlaylistWindow() => this.InitializeComponent();

		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			this.PlaylistElementStackPanel.Children.Clear();
			this.LoadButton.IsEnabled = false;
			this.DefaultButton.IsEnabled = false;

			String playlistUrl = this.InputTextBox.Text;

			if (!YoutubeClient.ValidatePlaylistId(playlistUrl) && !YoutubeClient.TryParsePlaylistId(playlistUrl, out playlistUrl))
			{
				MessageBox.Show("The supplied playlist url or id is syntactically incorrect!");
				this.LoadButton.IsEnabled = true;

				return;
			}

			try
			{
				Playlist playlist = await YouTubeClient.GetPlaylistAsync(playlistUrl);

				foreach (Video video in playlist.Videos)
					this.PlaylistElementStackPanel.Children.Add(new YouTubePlaylistElementControl(new InputMedia(video)));

			}
			catch (VideoUnavailableException ex)
			{
				MessageBox.Show(ex.Message);
			}
			catch (VideoRequiresPurchaseException ex)
			{
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				this.LoadButton.IsEnabled = false;
				this.DefaultButton.IsEnabled = true;
			}
		}

		private void ConfirmButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}
	}
}
