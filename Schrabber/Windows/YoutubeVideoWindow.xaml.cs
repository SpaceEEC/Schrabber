using Schrabber.Models;
using Schrabber.Workers;
using System;
using System.Windows;
using YoutubeExplode;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Videos;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for YoutubeVideoWindow.xaml
	/// </summary>
	public partial class YoutubeVideoWindow : Window
	{
		public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
			nameof(Media),
			typeof(Media),
			typeof(YoutubeVideoWindow),
			new PropertyMetadata(null)
		);

		public Media Media
		{
			get => (Media)this.GetValue(MediaProperty);
			set => this.SetValue(MediaProperty, value);
		}
		
		public YoutubeVideoWindow() => this.InitializeComponent();
		public YoutubeVideoWindow(Media media) : this() => this.Media = media;
		private void DefaultButton_Click(Object sender, RoutedEventArgs e) => this.DialogResult = true;

		private async void LoadButton_Click(Object sender, RoutedEventArgs e)
		{
			this.Media = null;
			this.LoadButton.IsEnabled = false;
			this.DefaultButton.IsEnabled = false;

			VideoId? videoId = VideoId.TryParse(this.InputTextBox.Text);
			try
			{
				if (!videoId.HasValue)
				{
					MessageBox.Show("The supplied video url or id is syntactically incorrect!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				Video video = await Youtube.Client.Videos.GetAsync(videoId.Value);
				if (video.Duration == null)
				{
					MessageBox.Show("Livestreams are not supported.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				this.Media = new YoutubeMedia(video);

				this.InputTextBox.Text = video.Url;

				this.DefaultButton.IsEnabled = true;
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
				this.LoadButton.IsEnabled = true;
			}
		}
	}
}
