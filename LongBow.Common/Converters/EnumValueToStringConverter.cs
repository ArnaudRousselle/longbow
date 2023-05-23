using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LongBow.Common.Converters
{
	public class EnumValueToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			var memInfo = value.GetType().GetMember(value.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), true);

			var descriptionAttribute = attributes.OfType<DescriptionAttribute>().FirstOrDefault();

			if (descriptionAttribute == null)
				return null;

			return descriptionAttribute.Description;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
