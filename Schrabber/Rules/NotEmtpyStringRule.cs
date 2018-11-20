using System;
using System.Globalization;
using System.Windows.Controls;

namespace Schrabber.Rules
{
	public class NotEmptyStringRule : ValidationRule
	{
		public override ValidationResult Validate(Object value, CultureInfo cultureInfo)
			=> new ValidationResult(!String.IsNullOrWhiteSpace(value as String), "This field may not be empty.");
	}
}
