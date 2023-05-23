using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.CalendarListing
{
	public interface ICalendarListingViewModel
	{
		double AverageMonthlyBalance { get; }
		ObservableCollection<RepetitiveBillingVom> RepetitiveBillings { get; }
		DelegateCommand<object> CreateBillingCommand { get; }
        DelegateCommand<IList> CreateMultipleBillingCommand { get; }
        DelegateCommand CreateRepetitiveBillingCommand { get; }
		DelegateCommand<object> EditRepetitiveBillingCommand { get; }
		DelegateCommand<object> DeleteRepetitiveBillingCommand { get; }
		InteractionRequest<IConfirmation> DeleteConfirmationRequest { get; }
	}
}
