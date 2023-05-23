using System.Collections.Generic;
using System.Linq;
using LongBow.Dom;
using LongBow.Dom.Constants;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.ObjectModel;

namespace LongBow.Listing
{
	public class DesignListingViewModel : IListingViewModel
	{
		private readonly ObservableCollection<BillingVom> _billings;

		public DesignListingViewModel()
		{
			_billings = new ObservableCollection<BillingVom>();

			for (var i = 1; i <= 20; i++)
			{
				_billings.Add(
					new BillingVom(
						new Billing
						{
							Id = i,
							TransactionDate = DateTime.Now.AddDays(-i - 1),
							ValuationDate = DateTime.Now.AddDays(-i),
							Title = "titre" + i,
							Amount = i + 50.2,
							Positive = i%3 == 0,
							Checked = i%4 == 0,
							Delayed = i%3 == 0,
							Comment = "commentaire " + i,
						}));
			}

			SelectedBillings = _billings
				.Where(n => n.BillingId == 1 || n.BillingId == 2)
				.ToList();
		}

	    public List<BillingVom> SelectedBillings { get; set; }

		public bool ShowArchived
		{
			get { return true; }
			set { }
		}

		public DateTime? TransactionDateStart
		{
			get { return DateTime.Now.AddMonths(-1); }
			set { }
		}

		public DateTime? TransactionDateEnd
		{
			get { return DateTime.Now; }
			set { }
		}

		public bool ShowOnlyNotChecked
		{
			get { return false; }
			set { }
		}

		public string ResearchedLabel
		{
			get { return "opération 56"; }
			set { }
		}

		public string ResearchedAmount
		{
			get { return "298,50"; }
			set { }
		}

		public string DeltaForResearchedAmount
		{
			get { return "1"; }
			set { }
		}

		public bool FiltersActivated
		{
			get { return true; }
		}

		public ObservableCollection<BillingVom> Billings
		{
			get { return _billings; }
		}

	    public DelegateCommand ClearFiltersCommand { get; set; }

		public DelegateCommand ClearLabelResearchCommand { get; set; }

		public DelegateCommand ClearAmountResearchCommand { get; set; }

		public DelegateCommand StartResearchCommand { get; set; }

		public DelegateCommand<object> CopyBillingsToClipboardCommand { get; set; }

		public DelegateCommand<object> SelectionChangedCommand { get; set; }

		public DelegateCommand<object> EditBillingCommand { get; set; }

		public DelegateCommand<object> CheckBillingCommand { get; set; }

		public DelegateCommand<object> DelayBillingCommand { get; set; }

		public DelegateCommand<object> DeleteBillingCommand { get; set; }

		public DelegateCommand<object> MergeBillingsCommand { get; set; }
		public DelegateCommand<object> ArchiveBillingCommand { get; set; }

		public InteractionRequest<IConfirmation> DeleteConfirmationRequest { get; set; }

		public bool IsExpanded { get; set; }

	}
}
