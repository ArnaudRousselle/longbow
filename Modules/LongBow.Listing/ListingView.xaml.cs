using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using LongBow.Common.Contracts;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.Listing
{
	[Export(ViewNames.ListingView), PartCreationPolicy(CreationPolicy.NonShared)]
	[ViewSortHint("0")]
	public partial class ListingView : UserControl
	{
		[ImportingConstructor]
		public ListingView(IListingViewModel viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();
		}
	}
}
