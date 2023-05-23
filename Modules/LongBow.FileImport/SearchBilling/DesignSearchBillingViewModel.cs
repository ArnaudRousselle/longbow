using System.Collections.ObjectModel;
using LongBow.Dom;
using Microsoft.Practices.Prism.Commands;

namespace LongBow.FileImport.SearchBilling
{
    public class DesignSearchBillingViewModel : ISearchBillingViewModel
    {
        public string ResearchedLabel { get; set; }

        public ObservableCollection<Billing> Billings { get; set; } = new ObservableCollection<Billing>
        {
            new Billing {Title = "Entrée 1"},
            new Billing {Title = "Entrée 2"},
            new Billing {Title = "Entrée 3"},
        };

        public DelegateCommand<Billing> ValidateCommand { get; set; }
    }
}
