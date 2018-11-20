using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Interaction logic for YoutubeVideoWindow.xaml
	/// </summary>
	public partial class YoutubeVideoWindow : Window
	{
		private static YoutubeClient _client = new YoutubeClient();

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

			String videoUrl = this.InputTextBox.Text;
			try
			{
				if (!YoutubeClient.ValidateVideoId(videoUrl) && !YoutubeClient.TryParseVideoId(videoUrl, out videoUrl))
				{
					MessageBox.Show("The supplied video url or id is syntactically incorrect!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				Video video = await _client.GetVideoAsync(videoUrl);
				if (video.Duration.Seconds == 0)
				{
					MessageBox.Show("Livestreams are not supported.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				this.Media = new YoutubeMedia(video);

				this.InputTextBox.Text = video.GetUrl();

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
