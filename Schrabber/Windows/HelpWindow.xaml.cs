using System;
using System.Windows;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class HelpWindow : Window
	{
		public readonly static DependencyProperty HelpTextDependencyProperty = DependencyProperty.Register(
			nameof(HelpText),
			typeof(String),
			typeof(HelpWindow)
		);

		public String HelpText
		{
			get => (String)this.GetValue(HelpWindow.HelpTextDependencyProperty);
			set => this.SetValue(HelpWindow.HelpTextDependencyProperty, value);
		}
	
		public HelpWindow(String helpText)
		{
			this.InitializeComponent();

			this.HelpText = helpText;
		}
	}
}
