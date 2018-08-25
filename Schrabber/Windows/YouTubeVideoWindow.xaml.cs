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
			InitializeComponent();
			DescriptionTextBox.Text = String.Empty;
			LabelDuration.Content = String.Empty;
			LabelTitle.Content = String.Empty;
		}

		private void Image_DownloadCompleted(object sender, EventArgs e)
		{
			BitmapImage image = (BitmapImage)sender;
			image.DownloadCompleted -= Image_DownloadCompleted;
			ThumbnailImage.Width = image.PixelWidth;

			DescriptionTextBox.Width = Math.Max(10, CenterGrid.ActualWidth - image.PixelWidth - 30);
		}

		private void Window_SizeChanged(object sender, RoutedEventArgs e)
			=> DescriptionTextBox.Width = Math.Max(10, CenterGrid.ActualWidth - ThumbnailImage.ActualWidth - 30);

		private void DefaultButton_Click(object sender, RoutedEventArgs e)
		{
				DialogResult = true;
				Close();
		}

		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			Media = null;
			LoadButton.IsEnabled = false;
			DefaultButton.IsEnabled = false;

			String videoUrl = InputTextBox.Text;

			if (!YoutubeClient.ValidateVideoId(videoUrl) && !YoutubeClient.TryParseVideoId(videoUrl, out videoUrl))
			{
				MessageBox.Show("The supplied video url or id is syntactically incorrect!");
				LoadButton.IsEnabled = false;

				return;
			}

			try
			{
				Video video = await YouTubeClient.GetVideoAsync(videoUrl);

				Media = new InputMedia(video);
				Media.CoverImage.DownloadCompleted += this.Image_DownloadCompleted;
				ThumbnailImage.Source = Media.CoverImage;

				LabelTitle.Content = video.Title;
				LabelDuration.Content = video.Duration.ToString();
				DescriptionTextBox.Text = video.Description;
				InputTextBox.Text = video.GetUrl();

				DefaultButton.IsEnabled = true;
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
			}
		}
	}
}