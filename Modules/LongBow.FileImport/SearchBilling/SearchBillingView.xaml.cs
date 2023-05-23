using Microsoft.Practices.ServiceLocation;

namespace LongBow.FileImport.SearchBilling
{
    public partial class SearchBillingView
    {
        public SearchBillingView()
        {
            if (ServiceLocator.IsLocationProviderSet)
                DataContext = ServiceLocator.Current.GetInstance<ISearchBillingViewModel>();

            InitializeComponent();
        }
    }
}
