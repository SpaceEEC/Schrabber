using Schrabber.Controls;
using Schrabber.Interfaces;
using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
				PlaylistElementStackPanel
					.Children
					.OfType<YouTubePlaylistElementControl>()
					.Where(c => c.Keep)
					.Select(c => c.Media);
		}
		public YouTubePlaylistWindow() => InitializeComponent();

		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			PlaylistElementStackPanel.Children.Clear();
			LoadButton.IsEnabled = false;
			DefaultButton.IsEnabled = false;

			String playlistUrl = InputTextBox.Text;

			if (!YoutubeClient.ValidatePlaylistId(playlistUrl) && !YoutubeClient.TryParsePlaylistId(playlistUrl, out playlistUrl))
			{
				MessageBox.Show("The supplied playlist url or id is syntactically incorrect!");
				LoadButton.IsEnabled = true;

				return;
			}

			try
			{
				Playlist playlist = await YouTubeClient.GetPlaylistAsync(playlistUrl);

				foreach (Video video in playlist.Videos)
					PlaylistElementStackPanel.Children.Add(new YouTubePlaylistElementControl(new InputMedia(video)));

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
				LoadButton.IsEnabled = false;
				DefaultButton.IsEnabled = true;
			}
		}

		private void ConfirmButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
