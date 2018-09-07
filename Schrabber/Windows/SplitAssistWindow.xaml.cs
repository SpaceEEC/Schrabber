using Schrabber.Extensions;
using Schrabber.Interfaces;
using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Interaction logic for SplitAssistWindow.xaml
	/// </summary>
	public partial class SplitAssistWindow : Window
	{
		public static DependencyProperty PartsProperty = DependencyProperty.Register(
			nameof(ListItems),
			typeof(ObservableCollection<IPart>),
			typeof(SplitAssistWindow),
			new PropertyMetadata(null)
		);
		public IEnumerable<IPart> Parts => this.ListItems.AsEnumerable();

		private ObservableCollection<IPart> ListItems
		{
			get => (ObservableCollection<IPart>)this.GetValue(PartsProperty);
			set => this.SetValue(PartsProperty, value);
		}

		public IInputMedia Media { get; }

		public SplitAssistWindow(IInputMedia media)
		{
			this.ListItems = new ObservableCollection<IPart>();
			this.Media = media;

			this.InitializeComponent();
		}

		private void PartsListBox_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
		{
			IPart original = (IPart)this.PartsListBox.SelectedItem;
			if (original == null) return;
			IPart copy = original.GetCopy();
			PartWindow window = new PartWindow()
			{
				Part = copy
			};
			if (window.ShowDialog() != true) return;

			this.ListItems.Remove(original);
			this.ListItems.Add(copy);
		}

		private static readonly Regex _createGroupsRegex = new Regex(@"\\\{\\\{(\w+)\}\}");

		private Tuple<String, Regex> GetTextAndRegex()
		{
			String text = this.DescriptionTextBox.SelectedText;
			if (String.IsNullOrWhiteSpace(text)) text = this.DescriptionTextBox.Text;
			if (String.IsNullOrWhiteSpace(text)) return new Tuple<String, Regex>(null, null);

			String escaped = Regex.Escape(this.InputTextBox.Text);
			String groupsRegexSource = _createGroupsRegex.Replace(escaped, (match) => $"(?<{match.Groups[1].Value}>.+?)") + "$";

			return new Tuple<String, Regex>(text, new Regex(groupsRegexSource, RegexOptions.Multiline));
		}

		private IEnumerable<Group> EnumerableGroups(String text, Regex regex)
		{
			foreach (Match match in regex.Matches(text))
			{
				foreach (Group group in match.Groups.Cast<Group>().Skip(1))
					yield return group;

				yield return null;
			}
		}

		private void SplitButton_Click(Object sender, RoutedEventArgs e)
		{
			(String text, Regex groupsRegex) = this.GetTextAndRegex();
			if (text == null) return;

			List<IPart> parts = new List<IPart>();

			IPart current = new PartViewModel(this.Media);
			foreach (Group group in this.EnumerableGroups(text, groupsRegex))
			{
				if (group == null)
				{
					parts.Add(current);
					current = new PartViewModel(this.Media);
					continue;
				}

				if (group.Value.Length <= 1) continue;

				switch (group.Name.Capitalize())
				{
					case nameof(IPart.Start):
						if (TimeSpan.TryParseExact(group.Value, new String[] { @"m\:s", @"h\:m\:s" }, System.Globalization.CultureInfo.InvariantCulture, out TimeSpan start))
						{
							current.Start = start;
						}
						else
						{
							MessageBox.Show($"Invalid timestamp for start \"{group.Value}\"!");
							return;
						}

						break;

					case nameof(IPart.Stop):
						if (TimeSpan.TryParseExact(group.Value, new String[] { @"m\:s", @"h\:m\:s" }, System.Globalization.CultureInfo.InvariantCulture, out TimeSpan stop))
						{
							current.Stop = stop;
						}
						else
						{
							MessageBox.Show($"Invalid timestamp for stop \"{group.Value}\"!");
							return;
						}

						break;

					case nameof(IPart.Title):
						current.Title = group.Value;
						break;

					case nameof(IPart.Author):
						current.Author = group.Value;
						break;

					case nameof(IPart.Album):
						current.Album = group.Value;
						break;
				}
			}

			foreach (IPart part in parts)
				this.ListItems.Add(part);
		}

		private void PreviewButton_Click(Object sender, RoutedEventArgs e)
		{
			(String text, Regex groupsRegex) = this.GetTextAndRegex();
			if (text == null) return;

			List<Run> runs = new List<Run>();

			Int32 pos = 0;
			foreach(Group group in this.EnumerableGroups(text, groupsRegex).Where(g => g != null))
			{
				String prefix = text.Slice(pos, group.Index);
				if (!String.IsNullOrEmpty(prefix)) this.AddRun(ref runs, prefix);
				this.AddRun(ref runs, group.Value, group.Name.Capitalize());

				pos = group.Index + group.Length;
			}

			String suffix = text.Substring(pos);
			if (!String.IsNullOrEmpty(suffix)) this.AddRun(ref runs, suffix);

			PreviewTextBlock.Inlines.Clear();
			PreviewTextBlock.Inlines.AddRange(runs);

			this.TabControl.SelectedIndex = 2;
		}

		private void AddRun(ref List<Run> runs, String value, String group = null)
		{
			Run run = new Run(value);
			switch (group)
			{
				case null:
					break;

				case nameof(IPart.Start):
				case nameof(IPart.Stop):
				case nameof(IPart.Title):
				case nameof(IPart.Author):
				case nameof(IPart.Album):
					run.Background = Brushes.Green;
					break;

				default:
					run.Background = Brushes.Red;
					break;
			}

			runs.Add(run);
		}

		private void RemoveAllButton_Click(Object sender, RoutedEventArgs e) => this.ListItems.Clear();

		private void DefaultButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;
	}
}
