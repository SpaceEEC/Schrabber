using Schrabber.Models;
using System;
using System.Collections.Generic;
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
	public partial class PartsGeneratorWindow : Window
	{
		public static readonly DependencyProperty MediaDependencyProperty = DependencyProperty.Register(
			nameof(Media),
			typeof(Media),
			typeof(PartsGeneratorWindow),
			new PropertyMetadata(null)
		);
		public Media Media
		{
			get => (Media)this.GetValue(MediaDependencyProperty);
			set => this.SetValue(MediaDependencyProperty, value);
		}

		public static readonly DependencyProperty HighlightRuleDependencyProperty = DependencyProperty.Register(
			nameof(HighlightRule),
			typeof(HighlightRule),
			typeof(PartsGeneratorWindow),
			new PropertyMetadata(null)
		);

		public HighlightRule HighlightRule
		{
			get => (HighlightRule)this.GetValue(HighlightRuleDependencyProperty);
			set => this.SetValue(HighlightRuleDependencyProperty, value);
		}

		public PartsGeneratorWindow(Media media)
		{
			this.InitializeComponent();
			
			this.Media = media;
			this.HighlightRule = new HighlightRule(Brushes.Green, @"\{.*?\}");

			this.htb.HighlightRules.Add(this.HighlightRule);
		}

		private void TextBox_TextChanged(Object sender, TextChangedEventArgs e)
		{
			this.HighlightRule.MatchText = ((TextBox)sender).Text;
			this.htb.ApplyHighlights();
		}
	}
}
