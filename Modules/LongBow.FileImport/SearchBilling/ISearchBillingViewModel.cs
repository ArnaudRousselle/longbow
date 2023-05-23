using System;
using System.Collections.ObjectModel;
using LongBow.Dom;
using Microsoft.Practices.Prism.Commands;

namespace LongBow.FileImport.SearchBilling
{
    public interface ISearchBillingViewModel
    {
        string ResearchedLabel { get; set; }
        ObservableCollection<Billing> Billings { get; }
        DelegateCommand<Billing> ValidateCommand { get; }
    }
}