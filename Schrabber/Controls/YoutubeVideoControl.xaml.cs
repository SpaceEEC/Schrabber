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
