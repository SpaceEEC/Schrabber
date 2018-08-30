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
		public static DependencyProperty ListItemsProperty = DependencyProperty.Register(
			nameof(ListItems),
			typeof(ObservableCollection<IInputMedia>),
			typeof(InputListWindow),
			new PropertyMetadata(null)
		);

		private ObservableCollection<IInputMedia> ListItems
		{
			get => (ObservableCollection<IInputMedia>)this.GetValue(ListItemsProperty);
			set => this.SetValue(ListItemsProperty, value);
		}

		private String _folderPath = null;

		public InputListWindow()
		{
			this.InitializeComponent();
			this.ListItems = new ObservableCollection<IInputMedia>();
		}
		private void PlaylistButton_Click(Object sender, RoutedEventArgs e)
		{
			YouTubePlaylistWindow window = new YouTubePlaylistWindow();
			if (window.ShowDialog() != true) return;


			foreach (IInputMedia media in window.Medias)
				this.ListItems.Add(media);
		}

		private void VideoButton_Click(Object sender, RoutedEventArgs e)
		{
			YouTubeVideoWindow window = new YouTubeVideoWindow();
			if (window.ShowDialog() != true) return;

			this.ListItems.Add(window.Media);
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
							this.ListItems.Add(media.Result);
					})
			);
		}

		#region SideGrid
		private async void StartButton_Click(Object sender, RoutedEventArgs e)
		{
			this.MainGrid.IsEnabled = false;

			IInputMedia[] media = this.ListItems.ToArray();

			ProgressWindow window = new ProgressWindow(media)
			{
				FolderPath = _folderPath,
			};
			window.Show();

			try
			{
				await window.Run();
			}
			catch (Exception ex)
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

			IDisposable[] disposables = this.ListItems.ToArray();
			this.ListItems.Clear();

			foreach (IDisposable disposable in disposables)
				disposable.Dispose();
		}
		#endregion SideGrid

		#region ElementGrid
		private void DeleteButton_Click(Object sender, RoutedEventArgs e)
		{
			IInputMedia media = (IInputMedia)((Button)sender).DataContext;
			this.ListItems.Remove(media);
			media.Dispose();
		}

		private void SplitButton_Click(Object sender, RoutedEventArgs e) => this._doSplit((IInputMedia)((Button)sender).DataContext);

		private void InputListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) => this._doSplit((IInputMedia)((ListBox)sender).SelectedItem);

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
