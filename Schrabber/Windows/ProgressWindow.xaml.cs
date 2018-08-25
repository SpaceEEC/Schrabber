using Schrabber.Interfaces;
using Schrabber.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for ProgressWindow.xaml
	/// </summary>
	public partial class ProgressWindow : Window, IProgressWindow
	{
		public Splitter Splitter { get; set; }

		private Int32 _totalMediaCount;
		public Int32 TotalMediaCount
		{
			get { return _totalMediaCount; }
			set
			{
				_totalMediaCount = value;
				Dispatcher.Invoke(() => MediaProgressBar.Maximum = value);
			}
		}

		private Double _progress = 0;
		public double Progress
		{
			get { return _progress; }
			set
			{
				_progress = value;
				Dispatcher.Invoke(() => StepProgressBar.Value = value * 100);
			}
		}

		private String _step = null;
		public String Step
		{
			get { return _step; }
			set
			{
				_step = value;
				Dispatcher.Invoke(() =>
				{
					StepLabel.Content = Step;
					if (Step != "Done") return;

					PartProgressBar.Value = PartProgressBar.Maximum;
					MediaProgressBar.Value = MediaProgressBar.Maximum;
					MediaLabel.Content = $"Video {TotalMediaCount} / {TotalMediaCount}";
				});
			}
		}

		private Int32 _currentPart = 0;
		private Int32 _currentMediaNumber = -1;

		private IInputMedia _currentMedia = null;
		public IInputMedia CurrentMedia
		{
			get
			{
				return _currentMedia;
			}
			set
			{
				_currentMedia = value;
				_currentPart = 0;

				Dispatcher.Invoke(() =>
				{
					PartProgressBar.Value = 0;
					PartLabel.Content = $"Part {_currentPart} / {_currentMedia.Parts.Length}";

					MediaProgressBar.Value = ++_currentMediaNumber;
					MediaLabel.Content = $"Video {MediaProgressBar.Value} / {TotalMediaCount}";
					PartProgressBar.Maximum = value.Parts.Length;

					ThumbnailImage.Source = value.CoverImage;
					DescriptionTextBox.Width = Math.Max(10, CenterGrid.ActualWidth - ThumbnailImage.ActualWidth - 30);

					ThumbnailImage.Width = value.CoverImage.PixelWidth;

					LabelTitle.Content = value.Title;
					LabelDuration.Content = value.Duration.ToString();

					DescriptionTextBox.Text = "Parts:\n\n";
					DescriptionTextBox.Text += String.Join("\n", value.Parts.Select(p =>
						String.IsNullOrWhiteSpace(p.Author)
							? $"{p.Start.ToString() ?? "00:00:00"} - {p.Title}"
							: $"{p.Start.ToString() ?? "00:00:00"} - {p.Author} - {p.Title}"
					));
				});
			}
		}

		public ProgressWindow() => InitializeComponent();

		public void NextPart()
		{
			Dispatcher.Invoke(() =>
			{
				PartProgressBar.Value = _currentPart++;
				PartLabel.Content = $"Part {_currentPart} / {_currentMedia.Parts.Length}";
			});
		}

		private void Window_SizeChanged(object sender, RoutedEventArgs e)
			=> DescriptionTextBox.Width = Math.Max(10, CenterGrid.ActualWidth - ThumbnailImage.ActualWidth - 30);

		private void OpenFolderButton_Click(object sender, RoutedEventArgs e) => Process.Start(Splitter.FolderPath);
	}
}
