using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Schrabber.Converters
{
	public class TabSizeConverter : IMultiValueConverter
	{
		// https://stackoverflow.com/a/804378
		public Object Convert(Object[] value, Type targetType, Object parameter, CultureInfo culture)
		{
			TabControl tabControl = (TabControl)value[0];
			Double width = tabControl.ActualWidth / tabControl.Items.Count;
			// leave 10 left and right empty
			return (width <= 20) ? 0 : (width - 20);
		}

		public Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
