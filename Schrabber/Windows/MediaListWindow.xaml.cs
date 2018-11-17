using Microsoft.Win32;
using Schrabber.Models;
using Schrabber.Workers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public ICommand RemoveItem { get; }

		public MediaListWindow()
		{
			this.InitializeComponent();
			this.ListItems = new ObservableCollection<Media>();

			this.RemoveItem = new DelegatedCommand<Media>(m => this.ListItems.Remove(m));
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
				this.IsEnabled = false;
				MessageBox.Show(
					"Can not find the FFmpeg executable.\nFFmpeg has to be installed and in the path.",
					"FFmpeg not found!",
					MessageBoxButton.OK,
					MessageBoxImage.Stop
				);
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
					MessageBox.Show(ex.ToString(), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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
	}
}
