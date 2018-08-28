using System;
using System.Globalization;
using System.Windows.Data;

namespace Schrabber.Converters
{
	public class EmptyStringToBooleanConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) => !String.IsNullOrWhiteSpace(value as String);
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
