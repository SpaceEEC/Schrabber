using Schrabber.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for ProgressWindow.xaml
	/// </summary>
	public partial class ProgressWindow : Window, IProgress<Double>
	{
		private Int32 _currentPart = 0;
		private Int32 _currentMediaNumber = -1;

		private IInputMedia _currentMedia = null;
		public IInputMedia CurrentMedia
		{
			get
			{
				return this._currentMedia;
			}
			set
			{
				this._currentMedia = value;
				this._currentPart = 0;

				this.Dispatcher.Invoke(() =>
				{
					this.PartProgressBar.Value = 0;
					this.PartLabel.Content = $"Part {this._currentPart} / {this.CurrentMedia.Parts.Length}";

					this.MediaProgressBar.Value = ++this._currentMediaNumber;
					this.MediaLabel.Content = $"Video {this.MediaProgressBar.Value} / {this._media.Length}";
					this.PartProgressBar.Maximum = value.Parts.Length;

					this.ThumbnailImage.Source = value.CoverImage;
					this.DescriptionTextBox.Width = Math.Max(10, this.CenterGrid.ActualWidth - this.ThumbnailImage.ActualWidth - 30);

					this.ThumbnailImage.Width = value.CoverImage.PixelWidth;

					this.LabelTitle.Content = value.Title;
					this.LabelDuration.Content = value.Duration.ToString();

					this.DescriptionTextBox.Text = "Parts:\n\n";
					this.DescriptionTextBox.Text += String.Join("\n", value.Parts.Select(p =>
						String.IsNullOrWhiteSpace(p.Author)
							? $"{p.Start.ToString() ?? "00:00:00"} - {p.Title}"
							: $"{p.Start.ToString() ?? "00:00:00"} - {p.Author} - {p.Title}"
					));
				});
			}
		}

		public ProgressWindow(IInputMedia[] media)
		{
			this.InitializeComponent();

			_media = media;

			this.MediaProgressBar.Maximum = _media.Length;
		}

		public void SetStep(String step)
		{
			this.Dispatcher.Invoke(() =>
			{
				this.StepLabel.Content = step;
				if (step != "Done") return;

				this.PartProgressBar.Value = this.PartProgressBar.Maximum;
				this.MediaProgressBar.Value = this.MediaProgressBar.Maximum;
				this.MediaLabel.Content = $"Video {this._media.Length} / {this._media.Length}";
			});
		}

		public void NextPart()
		{
			this.Dispatcher.Invoke(() =>
			{
				this.PartProgressBar.Value = this._currentPart++;
				this.PartLabel.Content = $"Part {this._currentPart} / {this.CurrentMedia.Parts.Length}";
			});
		}

		private void Window_SizeChanged(object sender, RoutedEventArgs e)
			=> this.DescriptionTextBox.Width = Math.Max(10, this.CenterGrid.ActualWidth - this.ThumbnailImage.ActualWidth - 30);

		private void OpenFolderButton_Click(object sender, RoutedEventArgs e) => Process.Start(this.FolderPath);

		void IProgress<Double>.Report(Double value) => Dispatcher.Invoke(() => this.StepProgressBar.Value = value * 100);
	}
}
