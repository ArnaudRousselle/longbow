using System;
using System.Collections.ObjectModel;
using LongBow.FileImport.SearchBilling;
using LongBow.FileImport.Services.BankPluginSelectionService;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.FileImport
{
	public enum TitleField
	{
		Label, Memo
	}
	public interface IFileImportViewModel
	{
        InteractionRequest<ISearchBillingConfirmation> SearchBillingRequest { get; }
		string BankId { get; set; }
		string BranchId { get; set; }
		string AccountId { get; set; }
		string Currency { get; set; }
		bool Delayed { get; set; }
		DateTime? ValuationDate { get; set; }
		TitleField TitleField { get; set; }
		ObservableCollection<DataLineVom> DataLines { get; }
		DelegateCommand ChooseFileCommand { get; }
		DelegateCommand RefreshCommand { get; }
		DelegateCommand<DataLineVom> ExecuteActionForDataLineCommand { get; }
		InteractionRequest<BankPluginSelectionNotification> BankPluginSelectionRequest { get; }
		InteractionRequest<Notification> NotificationRequest { get; }
	}
}
