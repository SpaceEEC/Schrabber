using System;
using System.Globalization;
using System.Windows.Data;

namespace Schrabber.Converters
{
	[ValueConversion(typeof(TimeSpan?), typeof(String))]
	public class TimeSpanConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			=> (value as TimeSpan? ?? TimeSpan.Zero).ToString(@"hh\:mm\:ss");

		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			String res = value as String;
			if (String.IsNullOrWhiteSpace(res)) return null;

			String[] inputs = new[] { @"hh\:mm\:ss", @"h\:mm\:ss", @"mm\:ss", @"m\:ss", @"ss", @"s" };
			if (TimeSpan.TryParseExact(res, inputs, CultureInfo.InvariantCulture, out TimeSpan ts)) return ts;

			throw new FormatException("Must be formatted as \"mm:ss\" or \"hh:mm:ss\".");
		}
	}
}
