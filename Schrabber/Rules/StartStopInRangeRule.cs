using Schrabber.Models;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Schrabber.Rules
{
	public class StartStopInRangeRule : ValidationRule
	{
		public override ValidationResult Validate(Object value, CultureInfo cultureInfo) => throw new NotSupportedException();

		public override ValidationResult Validate(Object value, CultureInfo cultureInfo, BindingGroup owner) => throw new NotSupportedException();

		public override ValidationResult Validate(Object value, CultureInfo cultureInfo, BindingExpressionBase owner)
		{
			String res = value as String;
			if (String.IsNullOrWhiteSpace(res)) return ValidationResult.ValidResult;

			String[] inputs = new[] { @"hh\:mm\:ss", @"h\:mm\:ss", @"mm\:ss", @"m\:ss", @"ss", @"s" };
			if (TimeSpan.TryParseExact(res, inputs, CultureInfo.InvariantCulture, out TimeSpan timeSpan))
			{ }
			else return new ValidationResult(false, "Must be formatted as \"mm:ss\" or \"hh:mm:ss\".");

			BindingExpression expr = (BindingExpression)owner;
			String name = expr.ResolvedSourcePropertyName;
			Part part = (Part)expr.ResolvedSource;

			if (name == nameof(Part.Start))
			{
				if (timeSpan > part.Parent.Duration) return new ValidationResult(false, $"\"{nameof(Part.Start)}\" must be less than {part.Parent.Duration.ToString(@"hh\:mm\:ss")}");
				if (timeSpan < TimeSpan.FromSeconds(0)) return new ValidationResult(false, $"\"{nameof(Part.Start)}\" may not be negative");
				if (timeSpan > part.Stop) return new ValidationResult(false, $"\"{nameof(Part.Start)}\" may not be greater than \"{nameof(Part.Stop)}\".");
			}
			else if (name == nameof(Part.Stop))
			{
				if (timeSpan > part.Parent.Duration) return new ValidationResult(false, $"\"{nameof(Part.Stop)}\" must be less than {part.Parent.Duration.ToString(@"hh\:mm\:ss")}");
				if (timeSpan < TimeSpan.FromSeconds(0)) return new ValidationResult(false, $"\"{nameof(Part.Stop)}\" may not be negative");
				if (timeSpan < part.Start) return new ValidationResult(false, $"\"{nameof(Part.Start)}\" may not be greater than \"{nameof(Part.Stop)}\"");
			}

			return ValidationResult.ValidResult;
		}
	}
}
