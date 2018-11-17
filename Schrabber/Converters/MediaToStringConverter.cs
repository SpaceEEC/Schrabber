using Schrabber.Models;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace Schrabber.Converters
{
	[ValueConversion(typeof(Media), typeof(String))]
	internal class MediaToStringConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			if (!(value is Media media)) return String.Empty;

			String res = media.ToString();

			if (media.Description != null) res += "\n" + Regex.Replace(media.Description, @"^\s*$\n|\r", String.Empty, RegexOptions.Multiline);

			return Regex.Match(res, @"(.*\n?\r?){0,6}").Value;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
