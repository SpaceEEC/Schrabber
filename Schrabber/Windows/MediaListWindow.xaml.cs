using Microsoft.Win32;
using Schrabber.Commands;
using Schrabber.Models;
using Schrabber.Workers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for MediaListWindow.xaml
	/// </summary>
	public partial class MediaListWindow : Window
	{
		public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register(
			nameof(ListItems),
			typeof(ObservableCollection<Media>),
			typeof(MediaListWindow),
			new PropertyMetadata(null)
		);

		private ObservableCollection<Media> ListItems
		{
			get => (ObservableCollection<Media>)this.GetValue(ListItemsProperty);
			set => this.SetValue(ListItemsProperty, value);
		}

		private String _folderPath = String.Empty;

		public ICommand RemoveItem { get; }

		public MediaListWindow()
		{
			this.InitializeComponent();
			this.ListItems = new ObservableCollection<Media>();

			this.RemoveItem = new DelegatedCommand<Media>(m =>
			{
				if (m is IDisposable disposable) disposable.Dispose();
				this.ListItems.Remove(m);
			});
		}

		private void VideoButton_Click(Object sender, RoutedEventArgs e)
		{
			YoutubeVideoWindow window = new YoutubeVideoWindow();
			if (window.ShowDialog() != true) return;

			this.ListItems.Add(window.Media);
		}

		private void Window_Loaded(Object sender, RoutedEventArgs e)
		{
			if (FFmpeg.FindExecutablePath() == null)
			{
				MessageBox.Show(
					"Can not find the FFmpeg executable.\nFFmpeg has to be installed and in the path for this application to work.",
					"FFmpeg not found!",
					MessageBoxButton.OK,
					MessageBoxImage.Stop
				);

				this.Close();
			}
		}

		private void FileButton_Click(Object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog()
			{
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
				Multiselect = true
			};

			if (ofd.ShowDialog() != true) return;

			foreach (String fileName in ofd.FileNames)
			{
				try
				{
					using (TagLib.File file = TagLib.File.Create(fileName))
						this.ListItems.Add(new LocalMedia(fileName, file));
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void PlaylistButton_Click(Object sender, RoutedEventArgs e)
		{
			YoutubePlayListWindow window = new YoutubePlayListWindow();
			if (window.ShowDialog() != true) return;

			foreach (Media media in window.ListItems)
				this.ListItems.Add(media);
		}

		private void SetFolderButton_Click(Object sender, RoutedEventArgs e)
		{
			// https://stackoverflow.com/a/50261723/10602948
			var dialog = new SaveFileDialog
			{
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
				Title = "Select a Directory",
				Filter = "Directory|*.this.directory",
				FileName = "select"
			};
			if (dialog.ShowDialog() != true) return;

			String path = this._folderPath = dialog.FileName
				.Replace("\\select.this.directory", "")
				.Replace(".this.directory", "");

			if (!System.IO.Directory.Exists(path))
				System.IO.Directory.CreateDirectory(path);
		}

		private void ResetButton_Click(Object sender, RoutedEventArgs e)
		{
			foreach (IDisposable disposable in this.ListItems.OfType<IDisposable>())
				disposable.Dispose();

			this.ListItems.Clear();
		}
	}
}
