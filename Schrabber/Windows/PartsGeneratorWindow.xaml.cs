using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
	/// Interaction logic for PartsGeneratorWindow.xaml
	/// </summary>
	public partial class PartsGeneratorWindow : Window, INotifyPropertyChanged
	{
		public static readonly DependencyProperty MediaDependencyProperty = DependencyProperty.Register(
			nameof(Media),
			typeof(Media),
			typeof(PartsGeneratorWindow),
			new PropertyMetadata(null)
		);

		public event PropertyChangedEventHandler PropertyChanged;

		public Media Media
		{
			get => (Media)this.GetValue(MediaDependencyProperty);
			set => this.SetValue(MediaDependencyProperty, value);
		}

		public String MatchText
		{
			get => this.HighlightRule.MatchText;
			set
			{
				this.HighlightRule.MatchText = value;
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.MatchText)));
				this.htb?.ApplyHighlights();
			}
		}
		public HighlightRule HighlightRule { get; set; } = new HighlightRule(Brushes.Green, String.Empty);

		public PartsGeneratorWindow(Media media)
		{
			this.InitializeComponent();
			
			this.Media = media;

			this.htb.HighlightRules.Add(this.HighlightRule);
		}
	}
}
