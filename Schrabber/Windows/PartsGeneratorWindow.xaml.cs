using Schrabber.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

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
								if (this._tryConvert(group.Value, out TimeSpan? start))
									part.Start = start;
								break;

							case nameof(Part.Stop):
								if (this._tryConvert(group.Value, out TimeSpan? stop))
									part.Stop = stop;
								break;
						}
					}

					return part;
				})
				.ToArray();
		}


		private void DefaultButton_Click(Object sender, RoutedEventArgs e)
			=> this.DialogResult = true;

		private HelpWindow _helpWindow = null;
		private void HelpButton_Click(Object sender, RoutedEventArgs e)
		{
			if (this._helpWindow == null)
			{
				this._helpWindow = new HelpWindow(Properties.Resources.PartsGeneratorWindow_Help);
				this._helpWindow.Closed += (s, e2) => this._helpWindow = null;
			}

			this._helpWindow.Show();
			this._helpWindow.Activate();
		}

		private Boolean _tryConvert(String str, out TimeSpan? ts)
		{
			try
			{
				ts = (TimeSpan)new Converters.TimeSpanConverter().ConvertBack(str, typeof(TimeSpan), null, CultureInfo.InvariantCulture);

				return true;
			}
			catch { }

			ts = null;
			return false;
		}

		private void Window_Closing(Object sender, CancelEventArgs e)
		{
			this._helpWindow?.Close();
			this._helpWindow = null;
		}
	}
}
