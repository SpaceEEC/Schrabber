using System;
using System.Globalization;
using System.Windows.Data;

namespace Schrabber.Converters
{
	[ValueConversion(typeof(Int32[]), typeof(String))]
	class FormatStringConverter : IMultiValueConverter
	{
		public Object Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture)
			=> String.Format((String)parameter, values);

		public Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
