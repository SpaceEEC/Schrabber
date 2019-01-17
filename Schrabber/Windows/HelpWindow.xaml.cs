using System;
using System.Collections.Generic;
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
