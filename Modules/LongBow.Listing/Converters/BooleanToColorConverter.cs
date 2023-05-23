using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LongBow.Listing.Converters
{
	public class BooleanToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool))
				return new SolidColorBrush(Colors.Transparent);

			var args = parameter as BooleanToColorConverterArgs;

			var changeColor = (bool) value;

			if (args != null && args.Inversion)
				changeColor = !changeColor;

			if (!changeColor)
				return new SolidColorBrush(Colors.Transparent);

			return new SolidColorBrush(args != null ? args.Color : Colors.Red);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BooleanToColorConverterArgs
	{
		public bool Inversion { get; set; }
		public Color Color { get; set; }
	}
}
