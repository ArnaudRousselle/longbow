using System.ComponentModel.Composition;
using System.Windows.Controls;
using LongBow.Common.Contracts;

namespace LongBow.RepetitiveBillingCreation
{
	[Export(ViewNames.RepetitiveBillingCreationView), PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class RepetitiveBillingCreationView : UserControl
	{
		[Import]
		public IRepetitiveBillingCreationViewModel ViewModel
		{
			get { return DataContext as IRepetitiveBillingCreationViewModel; }
			set { DataContext = value; }
		}

		public RepetitiveBillingCreationView()
		{
			InitializeComponent();
		}
	}
}
