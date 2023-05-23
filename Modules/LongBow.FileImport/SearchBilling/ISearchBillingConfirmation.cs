using LongBow.Dom;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.FileImport.SearchBilling
{
    public interface ISearchBillingConfirmation : IConfirmation
    {
        Billing Billing { get; set; }
    }
}
