using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using LongBow.Common.Enumerations;

namespace LongBow.Listing.Converters
{
	public class BillingsToSummaryConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			var collection = value as IList<BillingVom>;

			if (collection == null || collection.Count <= 1)
				return null;

			return new BillingsToSummaryConverterResult
			       {
				       Count = collection.Count,
				       Sum = collection.Sum(n => n.Amount*(n.Orientation == Orientation.Positive ? 1 : -1)),
				       Average = collection.Sum(n => n.Amount*(n.Orientation == Orientation.Positive ? 1 : -1))/collection.Count,
			       };
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BillingsToSummaryConverterResult
	{
		public int Count { get; set; }
		public double Sum { get; set; }
		public double Average { get; set; }
	}
}
