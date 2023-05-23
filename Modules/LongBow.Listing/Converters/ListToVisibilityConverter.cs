using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LongBow.Listing.Converters
{
	public class ListToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Visibility.Collapsed;

			var collection = value as IList;

			if (collection == null)
				return Visibility.Collapsed;

			return collection.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
