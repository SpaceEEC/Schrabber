using Schrabber.Interfaces;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Schrabber.Converters
{
	[ValueConversion(typeof(IInputMedia), typeof(String))]
	public class IInputMediaToStringConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			IInputMedia media = (IInputMedia)value;
			StringBuilder sb = new StringBuilder();
			if (!String.IsNullOrWhiteSpace(media.Author))
				sb.Append(media.Author).Append(" - ");

			sb.AppendLine(media.Title);

			if (media.Description != null)
			{
				sb
					.AppendLine()
					.Append(
						Regex.Replace(
							media.Description,
							@"^\s*$\n|\r",
							String.Empty,
							RegexOptions.Multiline
						).Trim()
					);
			}

			if (sb.Length > 200) sb.Length = 200;

			return sb.ToString();
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
