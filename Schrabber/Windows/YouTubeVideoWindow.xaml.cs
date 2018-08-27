using Schrabber.Helpers;
using Schrabber.Interfaces;
using Schrabber.Models;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using YoutubeExplode;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Models;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for YouTube.xaml
	/// </summary>
	public partial class YouTubeVideoWindow : Window
	{
		/// <summary>
		/// The from the user selected IInputMedia.
		/// </summary>
		public IInputMedia Media { get; private set; }

		public YouTubeVideoWindow()
		{
			this.InitializeComponent();
			this.DescriptionTextBox.Text = String.Empty;
			this.LabelDuration.Content = String.Empty;
			this.LabelTitle.Content = String.Empty;
		}

		private void Image_DownloadCompleted(object sender, EventArgs e)
		{
			BitmapImage image = (BitmapImage)sender;
			image.DownloadCompleted -= this.Image_DownloadCompleted;
			this.ThumbnailImage.Width = image.PixelWidth;

			this.DescriptionTextBox.Width = Math.Max(10, this.CenterGrid.ActualWidth - image.PixelWidth - 30);
		}

		private void Window_SizeChanged(object sender, RoutedEventArgs e)
			=> this.DescriptionTextBox.Width = Math.Max(10, this.CenterGrid.ActualWidth - this.ThumbnailImage.ActualWidth - 30);

		private void DefaultButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			this.Media = null;
			this.LoadButton.IsEnabled = false;
			this.DefaultButton.IsEnabled = false;

			String videoUrl = this.InputTextBox.Text;
			try
			{
				if (!YoutubeClient.ValidateVideoId(videoUrl) && !YoutubeClient.TryParseVideoId(videoUrl, out videoUrl))
				{
					MessageBox.Show("The supplied video url or id is syntactically incorrect!");
					return;
				}

				Video video = await YouTubeClient.GetVideoAsync(videoUrl);
				if (video.Duration.Seconds == 0)
				{
					MessageBox.Show("Livestreams are not supported.");
					return;
				}

				this.Media = new InputMedia(video);
				this.Media.CoverImage.DownloadCompleted += this.Image_DownloadCompleted;
				this.ThumbnailImage.Source = this.Media.CoverImage;

				this.LabelTitle.Content = video.Title;
				this.LabelDuration.Content = video.Duration.ToString();
				this.DescriptionTextBox.Text = video.Description;
				this.InputTextBox.Text = video.GetUrl();

				this.DefaultButton.IsEnabled = true;
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
				this.LoadButton.IsEnabled = true;
			}
		}
	}
}