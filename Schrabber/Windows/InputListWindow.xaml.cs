using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using Schrabber.Controls;
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
			String fileEndings = "*.webm;*.mpg;*.mp2;*.mpeg;*.mpe;*.mpv;*.ogg;*.mp4;*.m4p;*.m4v;*.mp3;*.m4a;*.aac;*.oga";
			OpenFileDialog ofd = new OpenFileDialog()
			{
				Filter = $"Video and Audio files ({fileEndings})|{fileEndings}|All files (*.*)|*.*;",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
				Multiselect = true,
			};

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
			StartButton.IsEnabled = InputElementStackPanel.Children.Count != 0;
		}

		private void AddChildElement(UIElement children)
		{
			InputElementStackPanel.Children.Add(children);
			StartButton.IsEnabled = true;
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
	}
}
