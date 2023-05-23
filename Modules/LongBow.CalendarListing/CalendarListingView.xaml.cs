using System.ComponentModel.Composition;
using System.Windows.Controls;
using LongBow.Common.Contracts;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.CalendarListing
{
	[Export(ViewNames.CalendarListingView), PartCreationPolicy(CreationPolicy.NonShared)]
	[ViewSortHint("1")]
	public partial class CalendarListingView : UserControl
	{
		[Import]
		public ICalendarListingViewModel ViewModel 
		{
			get { return DataContext as ICalendarListingViewModel; }
			set { DataContext = value; }
		}
		public CalendarListingView()
		{
			InitializeComponent();
		}
	}
}
