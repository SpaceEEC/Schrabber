using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
		public HighlightRule HighlightRule { get; set; } = new HighlightRule();

		public PartsGeneratorWindow(Media media)
		{
			this.Media = media;
			this.InitializeComponent();
			
		}

		public Part[] GetParts()
		{
			MatchCollection matches;
			try { matches = Regex.Matches(this.htb.Text, this.HighlightRule.MatchText, RegexOptions.Multiline); }
			catch { return null; }

			return matches
				.Cast<Match>()
				.Select(match =>
				{
					Part part = new Part(this.Media);
					foreach (Group group in match.Groups.Cast<Group>().Skip(1))
					{
						switch (group.Name.Capitalize())
						{
							case nameof(Part.Album):
								part.Album = group.Value;
								break;

							case nameof(Part.Author):
								part.Author = group.Value;
								break;

							case nameof(Part.Title):
								part.Title = group.Value;
								break;

							case nameof(Part.Start):
								part.Start = (TimeSpan)new Converters.TimeSpanConverter().ConvertBack(group.Value, typeof(TimeSpan), null, CultureInfo.InvariantCulture);
								break;

							case nameof(Part.Stop):
								part.Stop = (TimeSpan)new Converters.TimeSpanConverter().ConvertBack(group.Value, typeof(TimeSpan), null, CultureInfo.InvariantCulture);
								break;
						}
					}

					return part;
				})
				.ToArray();
		}


		private void DefaultButton_Click(Object sender, RoutedEventArgs e)
			=> this.DialogResult = true;
	}
}
