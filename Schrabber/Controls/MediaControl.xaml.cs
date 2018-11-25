using Schrabber.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Schrabber.Windows;

namespace Schrabber.Controls
{
	/// <summary>
	/// Interaction logic for MediaControl.xaml
	/// </summary>
	public partial class MediaControl : UserControl
	{
		public MediaControl() => this.InitializeComponent();

		private void DoSplit(Object sender, RoutedEventArgs e)
		{
			Media media = (Media)((FrameworkElement)sender).DataContext;
			PartListWindow window = new PartListWindow(media.GetCopy());
			if (window.ShowDialog() != true) return;

			media.Parts = window.GetParts(media);
		}

		#region Cover
		private void SetCover_Click(Object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog()
			{
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
				Multiselect = false
			};
			if (ofd.ShowDialog() != true) return;

			try
			{
				using (Stream stream = ofd.OpenFile())
					((Media)((FrameworkElement)sender).DataContext).SetBitmapImage(stream: stream);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void RemoveCover_Click(Object sender, RoutedEventArgs e)
			=> ((Media)((FrameworkElement)sender).DataContext).CoverImage = null;
		#endregion Cover

		#region RemoveItem(Property)
		public static readonly DependencyProperty RemoveItemProperty =
			DependencyProperty.Register(
			nameof(RemoveItem),
			typeof(ICommand),
			typeof(MediaControl),
			new UIPropertyMetadata(null)
		);

		public ICommand RemoveItem
		{
			get => (ICommand)this.GetValue(RemoveItemProperty);
			set => this.SetValue(RemoveItemProperty, value);
		}
		#endregion RemoveItem(Property)
	}
}
