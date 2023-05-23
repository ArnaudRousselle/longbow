using System;
using System.Collections.ObjectModel;
using LongBow.Dom;
using LongBow.FileImport.Objects;
using LongBow.FileImport.SearchBilling;
using LongBow.FileImport.Services.BankPluginSelectionService;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.FileImport
{
	public class DesignFileImportViewModel : IFileImportViewModel
	{
		public DesignFileImportViewModel()
		{
			DataLines = new ObservableCollection<DataLineVom>();

			for (var i = 0; i < 15; i++)
			{
			    var dataLineVom = new DataLineVom(new DataLine
			                                      {
			                                          AccountId = "xxx" + (i%2),
			                                          Name = "opération " + (i + 1),
			                                          Memo = "mémo " + (i + 1),
			                                          DtPosted = DateTime.Now.AddDays(-i),
			                                          FitId = "10015467831" + i,
			                                          TrnAmt = -50 - i,
			                                      });

				dataLineVom.Matchings.Add(new Matching(new Billing
				                                       {

														   Id = 2 * i + 1,
					                                       Title = "opération " + (2*i + 1),
					                                       ValuationDate = DateTime.Now.AddDays(-2*i + 1),
					                                       Checked = (2*i + 1)%3 == 0,
				                                       }, (2*i + 4)%3 == 0));

				dataLineVom.Matchings.Add(new Matching(new Billing
				                                       {
														   Id = 2 * i + 2,
					                                       Title = "opération " + (2*i + 2),
					                                       ValuationDate = DateTime.Now.AddDays(-2*i + 2),
					                                       Checked = (2*i + 2)%3 == 0,
				                                       }, (2*i + 3)%3 == 0));

				DataLines.Add(dataLineVom);
			}
		}

	    public InteractionRequest<ISearchBillingConfirmation> SearchBillingRequest { get; set; }

	    public string BankId
		{
			get { return "18065"; }
			set { }
		}

		public string BranchId
		{
			get { return "0000062"; }
			set { }
		}

		public string AccountId
		{
			get { return "15846317442"; }
			set { }
		}

		public string Currency
		{
			get { return "EUR"; }
			set { }
		}

		public bool Delayed { get; set; }
		public DateTime? ValuationDate { get; set; }

		public ObservableCollection<DataLineVom> DataLines { get; private set; }

		public DelegateCommand RefreshCommand { get; set; }
		public DelegateCommand ChooseFileCommand { get; set; }
		public DelegateCommand<DataLineVom> ExecuteActionForDataLineCommand { get; set; }
		public InteractionRequest<BankPluginSelectionNotification> BankPluginSelectionRequest { get; set; }
		public InteractionRequest<Notification> NotificationRequest { get; set; }
		public TitleField TitleField { get; set; }
	}
}
