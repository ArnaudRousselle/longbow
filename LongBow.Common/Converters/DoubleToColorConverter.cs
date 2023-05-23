using System;
using System.Windows.Data;
using System.Windows.Media;

namespace LongBow.Common.Converters
{
	public class DoubleToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var d = value as double?;

			if (d == null)
				return new SolidColorBrush(Color.FromRgb(0, 0, 0));

			return d.Value < 0
				? new SolidColorBrush(Color.FromRgb(200, 0, 0))
				: new SolidColorBrush(Color.FromRgb(0, 180, 0));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
