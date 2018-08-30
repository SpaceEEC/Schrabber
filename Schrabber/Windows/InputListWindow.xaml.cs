using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using Schrabber.Helpers;
using Schrabber.Interfaces;
using Schrabber.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for InputListWindow.xaml
	/// </summary>
	public partial class InputListWindow : Window
	{
		private String _folderPath = null;
		private ObservableCollection<IInputMedia> _listItems = new ObservableCollection<IInputMedia>();

		public InputListWindow()
		{
			this.InitializeComponent();
			this.InputListBox.ItemsSource = this._listItems;
		}
		private void PlaylistButton_Click(Object sender, RoutedEventArgs e)
		{
			YouTubePlaylistWindow window = new YouTubePlaylistWindow();
			if (window.ShowDialog() != true) return;


			foreach (IInputMedia media in window.Medias)
				this.AddMedia(media);
		}

		private void VideoButton_Click(Object sender, RoutedEventArgs e)
		{
			YouTubeVideoWindow window = new YouTubeVideoWindow();
			if (window.ShowDialog() != true) return;

			this.AddMedia(window.Media);
		}

		private void FileButton_Click(Object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = OpenFileDialogFactory.GetVideoAndMusicFileDialog();
			ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
			ofd.Multiselect = true;
			if (ofd.ShowDialog() != true) return;

			Task.Factory.ContinueWhenAll(

				ofd.FileNames.Select(fileName =>
						Task.Run(() =>
						{
							InputMedia media;
							// TODO: Catch exception
							using (TagLib.File file = TagLib.File.Create(fileName))
								media = new InputMedia(fileName, file);

							return (IInputMedia)media;
						})
				).ToArray(),

				medias =>
					this.Dispatcher.Invoke(() =>
					{
						foreach (Task<IInputMedia> media in medias)
							this.AddMedia(media.Result);
					})
			);
		}

		#region SideGrid
		private void RemoveMedia(IInputMedia media)
		{
			this._listItems.Remove(media);
			media.Dispose();
			this.StartButton.IsEnabled = this.ResetButton.IsEnabled = this._listItems.Count != 0;
		}

		private void AddMedia(IInputMedia media)
		{
			this._listItems.Add(media);
			this.StartButton.IsEnabled = true;
			this.ResetButton.IsEnabled = true;
		}

		private async void StartButton_Click(Object sender, RoutedEventArgs e)
		{
			this.MainGrid.IsEnabled = false;

			IInputMedia[] media = this._listItems.ToArray();

			ProgressWindow window = new ProgressWindow(media)
			{
				FolderPath = _folderPath,
			};
			window.Show();

			try
			{
				await window.Run();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
				System.Diagnostics.Debug.WriteLine(ex);
			}
			finally
			{
				this.MainGrid.IsEnabled = true;
			}
		}

		private void SetFolderButton_Click(Object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			if (dialog.ShowDialog() != true) return;

			this._folderPath = dialog.SelectedPath;
		}

		private void ResetButton_Click(Object sender, RoutedEventArgs e)
		{
			if (
				MessageBox.Show(
					"Are you sure you want to remove all elements?", "Confirmation",
					MessageBoxButton.YesNo, MessageBoxImage.Question,
					MessageBoxResult.No
				) != MessageBoxResult.Yes
			) return;

			IDisposable[] disposables = this._listItems.ToArray();
			this._listItems.Clear();

			foreach (IDisposable disposable in disposables)
				disposable.Dispose();

			this.StartButton.IsEnabled = false;
			this.ResetButton.IsEnabled = false;
		}
		#endregion SideGrid

		#region ElementGrid
		private void DeleteButton_Click(Object sender, RoutedEventArgs e) => this.RemoveMedia((IInputMedia)((Button)sender).DataContext);

		private void SplitButton_Click(Object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			IInputMedia media = (IInputMedia)button.DataContext;
			this._doSplit(media);
		}

		private void InputListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ListBox listBox = (ListBox)sender;
			this._doSplit((IInputMedia)listBox.SelectedItem);
		}

		private void _doSplit(IInputMedia media)
		{
			SplitWindow splitWindow = new SplitWindow(media);
			if (splitWindow.ShowDialog() != true) return;

			IPart[] parts = splitWindow.Parts.ToArray();
			for (Int32 i = 0; i > parts.Length; ++i)
			{
				if (i + 1 == parts.Length)
				{
					parts[i].Stop = media.Duration;
					break;
				}

				parts[i].Stop = parts[i + 1].Start;
			}

			media.Parts = parts;
		}
		#endregion ElementGrid

		#region CoverImageContextMenu
		private void SetCover_Click(Object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = OpenFileDialogFactory.GetImageFileDialog();
			ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

			if (ofd.ShowDialog() != true) return;

			using (FileStream fs = new FileInfo(ofd.FileName).OpenRead())
				((IInputMedia)((MenuItem)sender).DataContext).SetImage(fs);
		}
		private void RemoveCover_Click(Object sender, RoutedEventArgs e) => ((IInputMedia)((MenuItem)sender).DataContext).CoverImage = null;
		#endregion CoverImageContextMenu
	}
}
