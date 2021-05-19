using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Schrabber.Converters
{
	[ValueConversion(typeof(String), typeof(String))]
	public class SimpleToRegexConverter : IValueConverter
	{
		private static readonly Regex _createGroupsRegex = new Regex(@"\\\{\\\{(\w+?)\}\}");
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			return SimpleToRegexConverter._createGroupsRegex.Replace(
				Regex.Escape((String)value),
				match =>
				$"(?<{match.Groups[1].Value}>.+?)"
			) + "\\r?$";
		}

		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
