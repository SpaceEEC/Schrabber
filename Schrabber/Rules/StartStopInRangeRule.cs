using Schrabber.Interfaces;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Schrabber.Rules
{
	public class StartStopInRangeRule : ValidationRule
	{
		public StartStopInRangeRule() => this.ValidationStep = ValidationStep.UpdatedValue;

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			IPart part = (IPart)((BindingExpression)value).ResolvedSource;

			if (part.Start > part.Parent.Duration) return new ValidationResult(false, $"\"Start\" may not be greater than {part.Parent.Duration}");
			if (part.Start < TimeSpan.FromSeconds(0)) return new ValidationResult(false, $"\"Start\" may not be negative");

			if (part.Stop > part.Parent.Duration) return new ValidationResult(false, $"\"Stop\" may not be greater than {part.Parent.Duration}");
			if (part.Stop < TimeSpan.FromSeconds(0)) return new ValidationResult(false, $"\"Stop\" may not be negative");

			if (part.Start > part.Stop) return new ValidationResult(false, "\"Start\" may not be greater than \"Stop\".");

			return ValidationResult.ValidResult;
		}
	}
}
