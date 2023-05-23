using System.ComponentModel.Composition;
using System.Windows.Controls;
using LongBow.Common.Contracts;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.Reporting
{
	[Export(ViewNames.ReportingView), PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class ReportingView : UserControl
	{
		[Import]
		public IReportingViewModel ViewModel
		{
			get { return this.DataContext as IReportingViewModel; }
			set { this.DataContext = value; }
		}

		public ReportingView()
		{
			InitializeComponent();
		}
	}
}
