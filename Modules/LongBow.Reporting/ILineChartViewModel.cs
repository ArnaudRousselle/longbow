using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using OxyPlot;

namespace LongBow.Reporting
{
	public interface ILineChartViewModel
	{
		DateTime StartDate { get; set; }
		DateTime EndDate { get; set; }
		bool AllBillings { get; set; }
		bool ChartVisible { get; }
        List<PointItem> ChartItems { get; }
		Action RefreshChart { get; set; }
    }

	public class DataPoint
	{
		public string X { get; set; }
		public string Y { get; set; }
	}
}
