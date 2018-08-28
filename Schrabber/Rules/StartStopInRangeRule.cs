using Schrabber.Interfaces;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Schrabber.Rules
{
	public class StartStopInRangeRule : ValidationRule
	{
		public override ValidationResult Validate(Object value, CultureInfo cultureInfo) => throw new NotSupportedException();

		public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingGroup owner) => throw new NotSupportedException();

		public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingExpressionBase owner)
		{
			if (!TimeSpan.TryParse(value as String, out TimeSpan timeSpan)) return new ValidationResult(false, $"Not a valid timestamp \"{value}\"");

			BindingExpression expr = (BindingExpression)owner;
			String name = expr.ResolvedSourcePropertyName;
			IPart part = (IPart)expr.ResolvedSource;

			if (name == "Start")
			{
				if (timeSpan > part.Parent.Duration) return new ValidationResult(false, $"\"Start\" must be less than {part.Parent.Duration.ToString(@"hh\:mm\:ss")}");
				if (timeSpan < TimeSpan.FromSeconds(0)) return new ValidationResult(false, $"\"Start\" may not be negative");
				if (timeSpan > part.Stop) return new ValidationResult(false, "\"Start\" may not be greater than \"Stop\".");
			}
			else if (name == "Stop")
			{
				if (timeSpan > part.Parent.Duration) return new ValidationResult(false, $"\"Stop\" must be less than {part.Parent.Duration.ToString(@"hh\:mm\:ss")}");
				if (timeSpan < TimeSpan.FromSeconds(0)) return new ValidationResult(false, $"\"Stop\" may not be negative");
				if (timeSpan < part.Start) return new ValidationResult(false, "\"Start\" may not be greater than \"Stop\"");
			}

			return ValidationResult.ValidResult;
		}
	}
}
