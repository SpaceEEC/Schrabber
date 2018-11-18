using Schrabber.Commands;
using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Interaction logic for YoutubePlayListWindow.xaml
	/// </summary>
	public partial class YoutubePlayListWindow : Window
	{
		private readonly static YoutubeClient _client = new YoutubeClient();

		public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register(
			nameof(ListItems),
			typeof(ObservableCollection<Media>),
			typeof(YoutubePlayListWindow),
			new PropertyMetadata(null)
		);

		internal ObservableCollection<Media> ListItems
		{
			get => (ObservableCollection<Media>)this.GetValue(ListItemsProperty);
			set => this.SetValue(ListItemsProperty, value);
		}

		public ICommand RemoveItem { get; }

		public YoutubePlayListWindow()
		{
			this.InitializeComponent();
			this.ListItems = new ObservableCollection<Media>();
			this.RemoveItem = new DelegatedCommand<Media>(m =>
			{
				if (m is IDisposable disposable) disposable.Dispose();
				this.ListItems.Remove(m);
			});
		}

		private async void LoadButton_Click(Object sender, RoutedEventArgs e)
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
				Playlist playlist = await YoutubePlayListWindow._client.GetPlaylistAsync(playlistUrl);

				foreach (Video video in playlist.Videos)
					this.ListItems.Add(new YoutubeMedia(video));
			}
			catch (VideoUnavailableException ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (VideoRequiresPurchaseException ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				this.LoadButton.IsEnabled = false;
				this.DefaultButton.IsEnabled = true;
			}
		}

		private void DefaultButton_Click(Object sender, RoutedEventArgs e) => this.DialogResult = true;
	}
}
