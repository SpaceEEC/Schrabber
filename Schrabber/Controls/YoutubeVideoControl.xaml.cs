using Schrabber.Models;
using Schrabber.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Schrabber.Controls
{
	/// <summary>
	/// Interaction logic for YoutubeVideoControl.xaml
	/// </summary>
	public partial class YoutubeVideoControl : UserControl
	{
		public YoutubeVideoControl() => this.InitializeComponent();
		private void YoutubeVideoControl_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
			=> new YoutubeVideoWindow((Media)this.DataContext).ShowDialog();

		#region RemoveItem(Property)
		public static readonly DependencyProperty RemoveItemProperty =
			DependencyProperty.Register(
			nameof(RemoveItem),
			typeof(ICommand),
			typeof(YoutubeVideoControl),
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
