using Schrabber.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Schrabber.Windows
{
	// https://stackoverflow.com/a/42400991/10602948
	// Those changes are far from elegant
	public class HighlightTextBox : TextBox
	{
		// Using a DependencyProperty as the backing store for HighlightRules.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HighlightRuleProperty = DependencyProperty.Register(
				nameof(HighlightRule),
				typeof(HighlightRule),
				typeof(HighlightTextBox),
				new FrameworkPropertyMetadata(
					new HighlightRule(),
					new PropertyChangedCallback(HighlightRuleChanged)
				)
			);
		public HighlightRule HighlightRule
		{
			get => (HighlightRule)this.GetValue(HighlightRuleProperty);
			set => this.SetValue(HighlightRuleProperty, value);
		}

		public static readonly DependencyProperty MatchesDependencyProperty = DependencyProperty.Register(
			nameof(Matches),
			typeof(Int32),
			typeof(HighlightTextBox),
			new PropertyMetadata(0)
		);

		public Int32 Matches
		{
			get => (Int32)this.GetValue(MatchesDependencyProperty);
			set => this.SetValue(MatchesDependencyProperty, value);
		}

		private static void HighlightRuleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
			=> ((HighlightTextBox)sender).ApplyHighlights();

		public HighlightTextBox() : base()
		{
			this.Loaded += this.HighlightTextBox_Loaded;
			this.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(this.HighightTextBox_ScrollChanged));
			this.HighlightRule = new HighlightRule();
		}

		private void HighightTextBox_ScrollChanged(Object sender, ScrollChangedEventArgs e)
		{
			this.ApplyHighlights();
		}

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			base.OnTextChanged(e);
			this.ApplyHighlights();
		}

		private void HighlightTextBox_Loaded(Object sender, RoutedEventArgs e)
			=> this.ApplyHighlights();

		public void ApplyHighlights()
		{
			this.Matches = 0;
			this.TryRemoveAdorner<GenericAdorner>();

			if (String.IsNullOrEmpty(this.Text)) return;
			if (this.ActualHeight == 0 || this.ActualWidth == 0) return;

			MatchCollection matches;
			try { matches = Regex.Matches(this.Text, this.HighlightRule.MatchText, RegexOptions.Multiline); }
			catch { return; }

			this.Matches = matches.Count;

			foreach (Group group in matches.Cast<Match>().SelectMany(match => match.Groups.Cast<Group>().Skip(1)).Where(g => g.Length != 0))
			{
				Rect rect = this.GetRectFromCharacterIndex(group.Index);

				Rect backRect = this.GetRectFromCharacterIndex(group.Index + group.Length - 1, true);

				Brush brush;
				switch (group.Name.Capitalize())
				{
					case nameof(Part.Album):
					case nameof(Part.Author):
					case nameof(Part.Title):
						brush = Brushes.Green;

						break;

					case nameof(Part.Start):
					case nameof(Part.Stop):

						if (Regex.IsMatch(group.Value, @"((\d?\d:)?\d)?\d:\d\d *$"))
							brush = Brushes.Green;
						else
							brush = Brushes.Yellow;

						break;

					default:
						brush = Brushes.Red;

						break;
				}

				this.TryAddAdorner<GenericAdorner>(
					new GenericAdorner(
						this,
						new Rectangle() { Height = rect.Height, Width = backRect.X - rect.X, Fill = brush, Opacity = 0.5 },
						new Point(rect.X, rect.Y)
					)
				);
			}
		}
	}

	public class HighlightRule
	{
		public String MatchText { get; set; }

		public HighlightRule(String matchText)
		{
			this.MatchText = matchText;
		}
		public HighlightRule() : this(null) { }
	}

	public class GenericAdorner : Adorner
	{
		private readonly UIElement adorner;
		private readonly Point point;

		public GenericAdorner(UIElement targetElement, UIElement adorner, Point point) : base(targetElement)
		{
			this.adorner = adorner;
			if (adorner != null)
				this.AddVisualChild(adorner);

			this.point = point;
		}

		protected override Int32 VisualChildrenCount
		{
			get  => this.adorner == null ? 0 : 1;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			this.adorner?.Arrange(new Rect(this.point, this.adorner.DesiredSize));

			return finalSize;
		}
		protected override Visual GetVisualChild(Int32 index)
		{
			if (index == 0 && this.adorner != null)
				return this.adorner;

			return base.GetVisualChild(index);
		}
	}

	public static class Extensions
	{
		public static String Capitalize(this String str)
			=> Char.ToUpper(str[0]) + str.Substring(1).ToLower();

		public static void TryRemoveAdorner<T>(this UIElement element)
			where T : Adorner
			=> AdornerLayer.GetAdornerLayer(element)?.RemoveAdorners<T>(element);

		public static void RemoveAdorners<T>(this AdornerLayer layer, UIElement element)
			where T : Adorner
		{
			var adorners = layer.GetAdorners(element);
			if (adorners == null) return;
			foreach (T adorner in adorners.OfType<T>())
				layer.Remove(adorner);
		}

		public static void TryAddAdorner<T>(this UIElement element, Adorner adorner)
			where T : Adorner
		{
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);

			try { layer?.Add(adorner); }
			catch { }
		}

		public static Boolean HasAdorner<T>(this AdornerLayer layer, UIElement element)
			where T : Adorner
			=> layer.GetAdorners(element)?.OfType<T>().Any() ?? false;

		public static void RemoveAdorners(this AdornerLayer layer, UIElement element)
		{
			foreach (Adorner remove in layer?.GetAdorners(element) ?? new Adorner[0])
				layer.Remove(remove);
		}
	}
}