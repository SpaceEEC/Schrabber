using Schrabber.Commands;
using Schrabber.Models;
using Schrabber.Workers;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Playlists;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for YoutubePlayListWindow.xaml
	/// </summary>
	public partial class YoutubePlayListWindow : Window
	{
		public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register(
			nameof(ListItems),
			typeof(ObservableCollection<Media>),
			typeof(YoutubePlayListWindow),
			new PropertyMetadata(null)
		);

		public ObservableCollection<Media> ListItems
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

			PlaylistId? playlistId = PlaylistId.TryParse(this.InputTextBox.Text);

			if (!playlistId.HasValue)
			{
				MessageBox.Show("The supplied playlist url or id is syntactically incorrect!");
				this.LoadButton.IsEnabled = true;

				return;
			}

			try
			{
				


				foreach (PlaylistVideo video in await Youtube.Client.Playlists.GetVideosAsync(playlistId.Value))
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
