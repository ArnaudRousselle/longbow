using System.ComponentModel.Composition;
using System.Windows.Controls;
using LongBow.Common.Contracts;

namespace LongBow.BillingCreation
{
	[Export(ViewNames.EditingView), PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class EditingView : UserControl
	{
		[Import]
		public IEditingViewModel ViewModel
		{
			get { return DataContext as IEditingViewModel; }
			set { DataContext = value; }
		}

		public EditingView()
		{
			InitializeComponent();
		}
	}
}
