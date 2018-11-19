﻿using System;
using System.Collections.Generic;
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
	internal class HighlightTextBox : TextBox
	{
		internal List<HighlightRule> HighlightRules
		{
			get => (List<HighlightRule>)this.GetValue(HighlightRulesProperty);
			set => this.SetValue(HighlightRulesProperty, value);
		}

		// Using a DependencyProperty as the backing store for HighlightRules.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HighlightRulesProperty = DependencyProperty.Register(
			nameof(HighlightRules),
			typeof(List<HighlightRule>),
			typeof(HighlightTextBox),
			new FrameworkPropertyMetadata(
				new List<HighlightRule>(),
				new PropertyChangedCallback(HighlightRulesChanged)
				)
		);

		private static void HighlightRulesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
			=> ((HighlightTextBox)sender).ApplyHighlights();

		internal HighlightTextBox() : base()
			=> this.Loaded += this.HighlightTextBox_Loaded;

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			base.OnTextChanged(e);
			this.ApplyHighlights();
		}

		private void HighlightTextBox_Loaded(Object sender, RoutedEventArgs e)
			=> this.ApplyHighlights();

		internal void ApplyHighlights()
		{
			this.TryRemoveAdorner<GenericAdorner>();

			if (String.IsNullOrEmpty(this.Text)) return;
			if (this.ActualHeight == 0 || this.ActualWidth == 0) return;

			foreach (HighlightRule rule in this.HighlightRules.Where(rule => !String.IsNullOrEmpty(rule.MatchText)))
			{
				foreach (Match match in Regex.Matches(this.Text, rule.MatchText))
				{
					Rect rect = this.GetRectFromCharacterIndex(match.Index);

					Rect backRect = this.GetRectFromCharacterIndex(match.Index + match.Length - 1, true);

					this.TryAddAdorner<GenericAdorner>(
						new GenericAdorner(
							this,
							new Rectangle() { Height = rect.Height, Width = backRect.X - rect.X, Fill = rule.Brush, Opacity = 0.5 },
							new Point(rect.X, rect.Y)
						)
					);
				}
			}
		}
	}

	internal class HighlightRule
	{
		internal SolidColorBrush Brush { get; set; }
		internal String MatchText { get; set; }

		internal HighlightRule(SolidColorBrush solidColorBrush, String matchText)
		{
			this.Brush = solidColorBrush;
			this.MatchText = matchText;
		}
		internal HighlightRule(Color color, String matchText) : this(new SolidColorBrush(color), matchText) { }
		internal HighlightRule() : this(Brushes.Black, null) { }
	}

	internal class GenericAdorner : Adorner
	{
		private readonly UIElement adorner;
		private readonly Point point;

		internal GenericAdorner(UIElement targetElement, UIElement adorner, Point point) : base(targetElement)
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

	internal static class Extensions
	{
		internal static void TryRemoveAdorner<T>(this UIElement element)
			where T : Adorner
			=> AdornerLayer.GetAdornerLayer(element)?.RemoveAdorners<T>(element);

		internal static void RemoveAdorners<T>(this AdornerLayer layer, UIElement element)
			where T : Adorner
		{
			var adorners = layer.GetAdorners(element);
			if (adorners == null) return;
			foreach (T adorner in adorners.OfType<T>())
				layer.Remove(adorner);
		}

		internal static void TryAddAdorner<T>(this UIElement element, Adorner adorner)
			where T : Adorner
		{
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);

			try { layer?.Add(adorner); }
			catch { }
		}

		internal static Boolean HasAdorner<T>(this AdornerLayer layer, UIElement element)
			where T : Adorner
			=> layer.GetAdorners(element)?.OfType<T>().Any() ?? false;

		internal static void RemoveAdorners(this AdornerLayer layer, UIElement element)
		{
			foreach (Adorner remove in layer?.GetAdorners(element) ?? new Adorner[0])
				layer.Remove(remove);
		}
	}
}