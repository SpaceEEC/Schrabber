using Schrabber.Models;
using Schrabber.Workers;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace Schrabber.Converters
{
	[ValueConversion(typeof(Object), typeof(String))]
	public class TargetToStringConverter : IValueConverter
	{
		private static readonly String fmt = @"hh\:mm\:ss";
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			if (value == null) return String.Empty;

			if (value is Media media)
			{
				String album = String.IsNullOrEmpty(media.Album) ? String.Empty : $" [{media.Album}]";

				return $"{media.Duration.ToString(fmt)}{album}\n{media.ToString()}";
			}

			if (value is Part part)
			{
				Int32 index = Array.IndexOf(part.Parent.Parts, part);
				String start = part.Start?.ToString(fmt) ?? "00:00:00";
				String stop = part.Stop?.ToString(fmt) ?? part.Parent.Duration.ToString(fmt);
				String album = String.IsNullOrEmpty(part.Album) ? String.Empty : $" [{part.Album}]";

				return $"[{index + 1}/{part.Parent.Parts.Length}] -- {start} - {stop}{album}\n{part.ToString()}";
			}

			throw new NotSupportedException($"Can not convert from type {value.GetType().ToString()} to string.");
		}

		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
