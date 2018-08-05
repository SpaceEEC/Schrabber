using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using YoutubeExplode.Models;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for ProgressWindow.xaml
	/// </summary>
	public partial class ProgressWindow : Window, IDisposable
	{
		private Video _video;
		private Stream _stream;
		private List<Part> _parts;

		private String _caption = "Initial";
		private Double _progress = 0;

		// Local file
		public ProgressWindow(Stream stream, List<Part> parts)
		{
			InitializeComponent();

			_stream = stream;
		}

		// YouTube video
		public ProgressWindow(Video video, List<Part> parts)
		{
			InitializeComponent();

			_video = video;
			_parts = parts;

			BitmapImage image = new BitmapImage(new Uri(video.Thumbnails.HighResUrl));
			image.DownloadCompleted += (s, e) =>
			{
				ThumbnailImage.Width = image.PixelWidth;
				DescriptionTextBox.Width = Math.Max(10, CenterGrid.ActualWidth - image.PixelWidth - 10);
			};
			ThumbnailImage.Source = image;
			LabelTitle.Content = video.Title;
			LabelDuration.Content = video.Duration.ToString();

			DescriptionTextBox.Text = String.Join("\n", parts.Select(p =>
				String.IsNullOrWhiteSpace(p.Author)
					? $"{p.Timestamp.ToString()} - {p.Title}"
					: $"{p.Timestamp.ToString()} - {p.Author} - {p.Title}"
				)
			);
		}

		private void Window_SizeChanged(object sender, RoutedEventArgs e)
			=> DescriptionTextBox.Width = Math.Max(10, CenterGrid.ActualWidth - ThumbnailImage.ActualWidth - 10);

		public async Task Run()
		{
			Splitter splitter = new Splitter(_video, _parts)
			{
				UpdateCurrentAction = Update,
				UpdateCurrentProgress = Update
			};

			await splitter.Run();
		}

		private void Update()
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(Update);

				return;
			}

			String progress;
			if (_progress <= 1)
			{
				progress = $"{_caption} - {Math.Round(_progress * 100, 4)}%";
			}
			else
			{
				TimeSpan ts = TimeSpan.FromSeconds(_progress);
				String timeString = ts.Hours == 0 ? ts.ToString(@"mm\:ss") : ts.ToString(@"HH\:mm\:ss");
				progress = $"{_caption} - {timeString} processed";
			}

			ProgressLabel.Content = progress;
		}

		private void Update(String caption)
		{
			_caption = caption;
			Update();
		}

		private void Update(Double progress)
		{
			_progress = progress;
			Update();
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_stream.Dispose();
				}

				_video = null;
				_parts = null;

				disposedValue = true;
			}
		}

		public void Dispose() => Dispose(true);
		#endregion
	}
}
