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
	/// Interaction logic for YouTube.xaml
	/// </summary>
	public partial class YouTubeVideo : Window
	{
		public List<Part> Parts { get => _parts ?? new List<Part> { new Part() { Timestamp = TimeSpan.FromSeconds(0), Title = _video.Title } }; }

		private Video _video;
		private List<Part> _parts = null;

		private Boolean _modal = false;
		public YouTubeVideo()
		{
			InitializeComponent();
			DescriptionTextBox.Text = String.Empty;
			LabelDuration.Content = String.Empty;
			LabelTitle.Content = String.Empty;
		}

		public new bool? ShowDialog()
		{
			_modal = true;

			LoadButton.IsEnabled = false;
			InputTextBox.IsEnabled = false;

			DefaultButton.Content = "Confirm";
			return base.ShowDialog();
		}

		public void SetVideo(Video video)
		{
			_video = video;

			BitmapImage image = new BitmapImage(new Uri(_video.Thumbnails.HighResUrl));
			image.DownloadCompleted += this.Image_DownloadCompleted;
			ThumbnailImage.Source = image;

			LabelTitle.Content = _video.Title;
			LabelDuration.Content = _video.Duration.ToString();
			DescriptionTextBox.Text = _video.Description;
			InputTextBox.Text = _video.GetUrl();

			SplitButton.IsEnabled = true;
			DefaultButton.IsEnabled = true;
		}

		private void Image_DownloadCompleted(object sender, EventArgs e)
		{
			BitmapImage image = (BitmapImage)sender;
			image.DownloadCompleted -= Image_DownloadCompleted;
			ThumbnailImage.Width = image.PixelWidth;

			DescriptionTextBox.Width = Math.Max(10, CenterGrid.ActualWidth - image.PixelWidth - 10);
		}

		private void Window_SizeChanged(object sender, RoutedEventArgs e)
		{
			DescriptionTextBox.Width = Math.Max(10, CenterGrid.ActualWidth - ThumbnailImage.ActualWidth - 10);
		}

		private void SplitButton_Click(object sender, RoutedEventArgs e)
		{
			SplitVideoWindow splitVideoWindow = new SplitVideoWindow(DescriptionTextBox.SelectedText);
			if (splitVideoWindow.ShowDialog() ?? false)
				_parts = splitVideoWindow.Parts;
		}

		private async void DefaultButton_Click(object sender, RoutedEventArgs e)
		{
			if (_modal)
			{
				DialogResult = true;
				Close();

				return;
			}

			ProgressWindow progressWindow = new ProgressWindow(_video, Parts);
			progressWindow.Show();

			await progressWindow.Run();
		}


		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			_video = null;
			SplitButton.IsEnabled = false;

			String videoUrl = InputTextBox.Text;

			if (!YoutubeClient.ValidateVideoId(videoUrl) && !YoutubeClient.TryParseVideoId(videoUrl, out videoUrl))
			{
				MessageBox.Show("The supplied video url or id is syntactically incorrect!");

				return;
			}

			try
			{
				SetVideo(await YouTubeClient.GetVideoAsync(videoUrl));
			}
			catch (VideoUnavailableException ex)
			{
				MessageBox.Show(ex.Message);

				return;
			}
			catch (VideoRequiresPurchaseException ex)
			{
				MessageBox.Show(ex.Message);

				return;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

				return;
			}
		}
	}
}