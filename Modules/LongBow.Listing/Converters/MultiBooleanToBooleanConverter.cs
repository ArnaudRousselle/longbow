using System;
using System.Linq;
using System.Windows.Data;

namespace LongBow.Listing.Converters
{
	public class MultiBooleanToBooleanConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values == null)
				return false;

			return values
				.OfType<bool>()
				.All(n => n);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
