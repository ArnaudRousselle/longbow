using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.Listing
{
	public interface IListingViewModel
	{
		List<BillingVom> SelectedBillings { get; }
		bool ShowArchived { get; set; }
		DateTime? TransactionDateStart { get; set; }
		DateTime? TransactionDateEnd { get; set; }
		bool ShowOnlyNotChecked { get; set; }
		string ResearchedLabel { get; set; }
		string ResearchedAmount { get; set; }
		string DeltaForResearchedAmount { get; set; }
		bool FiltersActivated { get; }
		bool IsExpanded { get; set; }
		ObservableCollection<BillingVom> Billings { get; }
		DelegateCommand ClearFiltersCommand { get; }
		DelegateCommand ClearLabelResearchCommand { get; }
		DelegateCommand ClearAmountResearchCommand { get; }
		DelegateCommand StartResearchCommand { get; }
		DelegateCommand<object> CopyBillingsToClipboardCommand { get; }
		DelegateCommand<object> SelectionChangedCommand { get; }
		DelegateCommand<object> EditBillingCommand { get; }
		DelegateCommand<object> CheckBillingCommand { get; }
		DelegateCommand<object> DelayBillingCommand { get; }
		DelegateCommand<object> DeleteBillingCommand { get; }
		DelegateCommand<object> MergeBillingsCommand { get; }
		DelegateCommand<object> ArchiveBillingCommand { get; }
		InteractionRequest<IConfirmation> DeleteConfirmationRequest { get; }
	}
}