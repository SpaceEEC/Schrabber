using System;
using System.Globalization;
using System.Windows.Data;

namespace Schrabber.Converters
{
	[ValueConversion(typeof(Int32), typeof(Boolean))]
	public class IsNotZeroConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			=> (Int32)value != 0;

		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
