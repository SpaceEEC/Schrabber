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
		public static DependencyProperty ListItemsProperty = DependencyProperty.Register(
			nameof(ListItems),
			typeof(ObservableCollection<IInputMedia>),
			typeof(YouTubePlaylistWindow),
			new PropertyMetadata(null)
		);

		/// <summary>
		/// IEnumerable of to be imported IInputMedias.
		/// </summary>
		public IEnumerable<IInputMedia> Medias => this.ListItems.AsEnumerable();

		private ObservableCollection<IInputMedia> ListItems
		{
			get => (ObservableCollection<IInputMedia>)this.GetValue(ListItemsProperty);
			set => this.SetValue(ListItemsProperty, value);
		}

		public YouTubePlaylistWindow()
		{
			this.InitializeComponent();
			this.ListItems = new ObservableCollection<IInputMedia>();
		}
		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			this.ListItems.Clear();
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
					this.ListItems.Add(new InputMedia(video));
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

		private void VideoRemoveButton_Click(Object sender, EventArgs e) => this.ListItems.Remove((IInputMedia)((Button)sender).DataContext);
	}
}
