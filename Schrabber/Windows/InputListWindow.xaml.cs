using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using Schrabber.Controls;
using Schrabber.Helpers;
using Schrabber.Interfaces;
using Schrabber.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for InputListWindow.xaml
	/// </summary>
	public partial class InputListWindow : Window, IRemoveChildElement
	{
		private String _folderPath = null;

		public InputListWindow() => InitializeComponent();

		private void PlaylistButton_Click(object sender, RoutedEventArgs e)
		{
			YouTubePlaylistWindow window = new YouTubePlaylistWindow();
			if (window.ShowDialog() != true) return;


			foreach (IInputMedia media in window.SelectedMedias)
				AddChildElement(new InputElementControl(media, this));
		}

		private void VideoButton_Click(object sender, RoutedEventArgs e)
		{
			YouTubeVideoWindow window = new YouTubeVideoWindow();
			if (window.ShowDialog() != true) return;


			AddChildElement(new InputElementControl(window.Media, this));
		}

		private void FileButton_Click(object sender, RoutedEventArgs e)
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
					Dispatcher.Invoke(() =>
					{
						foreach (Task<IInputMedia> media in medias)
							AddChildElement(new InputElementControl(media.Result, this));
					})
			);
		}

		void IRemoveChildElement.RemoveChildElement(UIElement children)
		{
			if (children is IDisposable disposable) disposable.Dispose();

			InputElementStackPanel.Children.Remove(children);
			StartButton.IsEnabled = ResetButton.IsEnabled = InputElementStackPanel.Children.Count != 0;
		}

		private void AddChildElement(UIElement children)
		{
			InputElementStackPanel.Children.Add(children);
			StartButton.IsEnabled = true;
			ResetButton.IsEnabled = true;
		}

		private async void StartButton_Click(object sender, RoutedEventArgs e)
		{
			MainGrid.IsEnabled = false;

			IInputMedia[] media = InputElementStackPanel
				.Children
				.OfType<InputElementControl>()
				.Select(c => c.Media)
				.ToArray();

			ProgressWindow window = new ProgressWindow();
			window.Show();

			Splitter splitter = new Splitter(media, window)
			{
				FolderPath = _folderPath,
			};
			try
			{
				await splitter.Run();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
				System.Diagnostics.Debug.WriteLine(ex);
			}
			finally
			{
				MainGrid.IsEnabled = true;
			}
		}

		private void SetFolderButton_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			if (dialog.ShowDialog() != true) return;

			_folderPath = dialog.SelectedPath;
		}

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			if (
				MessageBox.Show(
					"Are you sure you want to remove all elements?", "Confirmation",
					MessageBoxButton.YesNo, MessageBoxImage.Question,
					MessageBoxResult.No
				) != MessageBoxResult.Yes
			) return;

			foreach (IDisposable disposable in InputElementStackPanel.Children.OfType<IDisposable>())
				disposable.Dispose();

			InputElementStackPanel.Children.Clear();
			StartButton.IsEnabled = false;
			ResetButton.IsEnabled = false;
		}
	}
}
