using System;
using System.Collections.ObjectModel;
using LongBow.Common.Enumerations;
using LongBow.Dom;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.BillingCreation
{
	public interface IEditingViewModel
	{
		int? EditingBillingId { get; }
		int? RepetitiveBillingId { get; }
		DateTime TransactionDate { get; set; }
		DateTime ValuationDate { get; set; }
		string Title { get; set; }
		string Amount { get; set; }
		Orientation Orientation { get; set; }
		bool Checked { get; set; }
		bool Delayed { get; set; }
		string Comment { get; set; }
		bool ShiftValuationDate { get; set; }
		
		DelegateCommand ValidateCommand { get; }
		InteractionRequest<IConfirmation> CloseConfirmationRequest { get; }
		DelegateCommand SwitchTabCommand { get; }
	}
}
