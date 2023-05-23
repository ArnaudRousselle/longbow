using System.ComponentModel.Composition;
using System.Windows.Controls;
using LongBow.Common.Contracts;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.Reporting
{
	[Export(ViewNames.LineChartView)]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	[ViewSortHint("5")]
	public partial class LineChartView : UserControl
	{
		[Import]
		public ILineChartViewModel ViewModel
		{
			get { return DataContext as ILineChartViewModel; }
		    set
		    {

		        DataContext = value;
		        value.RefreshChart = () =>
		                             {
                                         Chart.InvalidatePlot();
		                             };
		    }
		}

		public LineChartView()
		{
			InitializeComponent();
		}
	}
}
