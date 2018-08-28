using System;
using System.Globalization;
using System.Windows.Data;

namespace Schrabber.Converters
{
	public class IndexToBooleanConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) => (value as Int32? ?? -1) != -1;
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
