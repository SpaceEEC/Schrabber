using Schrabber.Helpers;
using Schrabber.Interfaces;
using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
		/// IEnumerable of to be imported IInputMedias.
		/// </summary>
		public IEnumerable<IInputMedia> Medias => this._listItems.AsEnumerable();

		private ObservableCollection<IInputMedia> _listItems = new ObservableCollection<IInputMedia>();

		public YouTubePlaylistWindow()
		{
			this.InitializeComponent();
			this.VideosListBox.ItemsSource = _listItems;
		}

		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			this._listItems.Clear();
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
					this._listItems.Add(new InputMedia(video));
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

		private void ConfirmButton_Click(Object sender, RoutedEventArgs e) => this.DialogResult = true;

		private void VideosListBox_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
		{
			if (!(((ListBox)sender).SelectedItem is IInputMedia media)) return;
			new YouTubeVideoWindow(media).ShowDialog();
		}

		private void VideoRemoveButton_Click(Object sender, EventArgs e) => this._listItems.Remove((IInputMedia)((Button)sender).DataContext);
	}
}
