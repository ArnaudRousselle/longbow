using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace LongBow.Common.Converters
{
	public class ValidationErrorsToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var observableCollection = value as ReadOnlyObservableCollection<ValidationError>;

			return observableCollection != null
				? string.Join(" / ", observableCollection.Select(n => n.ErrorContent))
				: null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
