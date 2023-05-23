using LongBow.Dom;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.FileImport.SearchBilling
{
    public class SearchBillingConfirmation : Confirmation, ISearchBillingConfirmation
    {
        public Billing Billing { get; set; }

        public SearchBillingConfirmation()
        {
            Title = "Chercher une opération";
        }
    }
}