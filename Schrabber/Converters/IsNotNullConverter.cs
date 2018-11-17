using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Schrabber.Converters
{
	[ValueConversion(typeof(Object), typeof(Boolean))]
	internal class IsNotNullConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) => value != null;
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
