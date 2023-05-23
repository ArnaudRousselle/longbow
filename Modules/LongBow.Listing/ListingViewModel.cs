using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using LongBow.Common.Enumerations;
using LongBow.Common.EventMessages;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.Interfaces.Tabulation;
using System.Collections.ObjectModel;
using LongBow.Common.Contracts;
using LongBow.Common.Interfaces.Views;
using LongBow.Common.Regions;
using LongBow.Dom;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Orientation = LongBow.Common.Enumerations.Orientation;

namespace LongBow.Listing
{
	//todo il y a une erreur dans la console au chargement du ViewModel : Cannot find governing FrameworkElement or FrameworkContentElement for target element. BindingExpression:Path=DataContext.StartResearchCommand; DataItem='ListingView' (Name='ListingViewUserControl');
	//todo il y a une erreur dans la console au chargement du ViewModel : Cannot find source for binding with reference 'ElementName=ListingViewUserControl'. BindingExpression:Path=DataContext.StartResearchCommand; DataItem='ListingView' (Name='ListingViewUserControl'); target element is 'KeyBinding' (HashCode=33421636); target property is 'Command' (type 'ICommand')
	[Export(typeof(IListingViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class ListingViewModel : BindableBase, IListingViewModel, IClosable, ITab, ISpecificCommands, IViewRemovedAware
	{
		~ListingViewModel()
		{
			_loggerFacade.Log("ListingViewModel garbage collected", Category.Debug, Priority.Low);
		}

		private readonly ILoggerFacade _loggerFacade;
		private readonly IBusinessContext _businessContext;
		private readonly IRegionManager _regionManager;

		private readonly List<SubscriptionToken> _subscriptionTokens = new List<SubscriptionToken>();
		private List<BillingVom> _selectedBillings;
		private bool _showArchived;
		private bool _showOnlySaving;
		private DateTime? _transactionDateStart;
		private DateTime? _transactionDateEnd;
		private bool _showOnlyNotChecked;
		private string _researchedLabel;
		private string _researchedAmount;
		private string _deltaForResearchedAmount = "0";
		private bool _isExpanded;
		private readonly ICollectionView _billingsCollectionView;
		private DelegateCommand _clearFiltersCommand;
		private DelegateCommand _clearLabelResearchCommand;
		private DelegateCommand _clearAmountResearchCommand;
		private DelegateCommand _startResearchCommand;
		private DelegateCommand<object> _checkBillingCommand;
		private DelegateCommand<object> _delayBillingCommand;
		private DelegateCommand<object> _copyBillingsToClipboardCommand;
		private DelegateCommand<object> _selectionChangedCommand;
		private DelegateCommand<object> _editBillingCommand;
		private DelegateCommand<object> _deleteBillingCommand;
		private DelegateCommand<object> _mergeBillingsCommand;
		private DelegateCommand<object> _archiveBillingCommand;
		private InteractionRequest<IConfirmation> _deleteConfirmationRequest;
		private DelegateCommand _closeTabCommand;



		[ImportingConstructor]
		public ListingViewModel(IBusinessContext businessContext,
			IRegionManager regionManager,
			IEventAggregator eventAggregator,
			ILoggerFacade loggerFacade)
		{
			_businessContext = businessContext;
			_regionManager = regionManager;
			_loggerFacade = loggerFacade;

			Billings = new ObservableCollection<BillingVom>(
				_businessContext
					.BillingManager
					.GetBillings()
					.Select(n => new BillingVom(n)));

			_billingsCollectionView = CollectionViewSource.GetDefaultView(Billings);

			_billingsCollectionView.Filter = FilterBillings;
			_billingsCollectionView.SortDescriptions.Add(new SortDescription("ValuationDate", ListSortDirection.Ascending));
			_billingsCollectionView.SortDescriptions.Add(new SortDescription("TransactionDate", ListSortDirection.Ascending));

			#region Events subscriptions

			_subscriptionTokens.Add(
				eventAggregator
				.GetEvent<AddedBillingEvent>()
				.Subscribe(args =>
						   {
							   var newBillingVom = new BillingVom(args.AddedBilling);

							   Billings.Add(newBillingVom);

							   _billingsCollectionView.MoveCurrentTo(newBillingVom);
						   }));

			_subscriptionTokens.Add(
				eventAggregator
				.GetEvent<UpdatedBillingEvent>()
				.Subscribe(args =>
						   {
							   var billingToUpdate = Billings
								   .FirstOrDefault(n => n.BillingId == args.UpdatedBilling.Id);

							   if (billingToUpdate == null)
								   return;

							   var index = Billings.IndexOf(billingToUpdate);

							   Billings.RemoveAt(index);

							   var newBillingVom = new BillingVom(args.UpdatedBilling);

							   Billings.Insert(index, newBillingVom);

							   _billingsCollectionView.MoveCurrentTo(newBillingVom);
						   }));

			_subscriptionTokens.Add(
				eventAggregator
				.GetEvent<DeletedBillingEvent>()
				.Subscribe(args =>
						   {
							   var billing = Billings.FirstOrDefault(n => n.BillingId == args.BillingId);

							   if (billing != null)
								   Billings.Remove(billing);
						   }));

			_subscriptionTokens.Add(
				eventAggregator
				.GetEvent<UpdateCheckedBillingEvent>()
				.Subscribe(args =>
						   {
							   var billing = Billings.FirstOrDefault(n => n.BillingId == args.BillingId);

							   if (billing != null)
								   billing.Checked = args.Checked;
						   }));

			_subscriptionTokens.Add(
				eventAggregator
				.GetEvent<UpdateDelayedBillingEvent>()
				.Subscribe(args =>
						   {
							   var billing = Billings.FirstOrDefault(n => n.BillingId == args.BillingId);

							   if (billing != null)
								   billing.Delayed = args.Delayed;
						   }));

			#endregion
		}

		public List<BillingVom> SelectedBillings
		{
			get { return _selectedBillings; }
			set
			{
				SetProperty(ref _selectedBillings, value);

				foreach (var simpleCommand in Commands)
				{
					simpleCommand.Command.RaiseCanExecuteChanged();
				}
			}
		}

		public bool ShowArchived
		{
			get { return _showArchived; }
			set
			{
				SetProperty(ref _showArchived, value);
				_billingsCollectionView.Refresh();
				OnPropertyChanged(() => FiltersActivated);
			}
		}

		public bool ShowOnlySaving
		{
			get { return _showOnlySaving; }
			set
			{
				SetProperty(ref _showOnlySaving, value);
				_billingsCollectionView.Refresh();
				OnPropertyChanged(() => FiltersActivated);
			}
		}

		public DateTime? TransactionDateStart
		{
			get { return _transactionDateStart; }
			set
			{
				SetProperty(ref _transactionDateStart, value);
				_billingsCollectionView.Refresh();
				OnPropertyChanged(() => FiltersActivated);
			}
		}

		public DateTime? TransactionDateEnd
		{
			get { return _transactionDateEnd; }
			set
			{
				SetProperty(ref _transactionDateEnd, value);
				_billingsCollectionView.Refresh();
				OnPropertyChanged(() => FiltersActivated);
			}
		}

		public bool ShowOnlyNotChecked
		{
			get { return _showOnlyNotChecked; }
			set
			{
				SetProperty(ref _showOnlyNotChecked, value);
				_billingsCollectionView.Refresh();
				OnPropertyChanged(() => FiltersActivated);
			}
		}

		public string ResearchedLabel
		{
			get { return _researchedLabel; }
			set
			{
				SetProperty(ref _researchedLabel, value);
				_billingsCollectionView.Refresh();
				OnPropertyChanged(() => FiltersActivated);
			}
		}

		public string ResearchedAmount
		{
			get { return _researchedAmount; }
			set
			{
				SetProperty(ref _researchedAmount, value);
				_billingsCollectionView.Refresh();
				OnPropertyChanged(() => FiltersActivated);
			}
		}
		public string DeltaForResearchedAmount
		{
			get { return _deltaForResearchedAmount; }
			set
			{
				SetProperty(ref _deltaForResearchedAmount, value);
				_billingsCollectionView.Refresh();
			}
		}

		public bool FiltersActivated
		{
			get
			{
				return _showArchived
					|| _showOnlySaving
					   || _transactionDateStart != null
					   || _transactionDateEnd != null
					   || _showOnlyNotChecked
					   || !string.IsNullOrWhiteSpace(_researchedLabel)
					   || !string.IsNullOrWhiteSpace(_researchedAmount);
			}
		}

		public bool IsExpanded
		{
			get { return _isExpanded; }
			set { SetProperty(ref _isExpanded, value); }
		}

		public ObservableCollection<BillingVom> Billings { get; private set; }

		public DelegateCommand ClearFiltersCommand
		{
			get
			{
				return _clearFiltersCommand ??
					   (_clearFiltersCommand = new DelegateCommand(() =>
					   {
						   _showArchived = false;
						   _showOnlySaving = false;
						   _transactionDateStart = null;
						   _transactionDateEnd = null;
						   _showOnlyNotChecked = false;
						   _researchedLabel = string.Empty;
						   _researchedAmount = string.Empty;

						   OnPropertyChanged(() => ShowArchived);
						   OnPropertyChanged(() => ShowOnlySaving);
						   OnPropertyChanged(() => TransactionDateStart);
						   OnPropertyChanged(() => TransactionDateEnd);
						   OnPropertyChanged(() => ShowOnlyNotChecked);
						   OnPropertyChanged(() => ResearchedLabel);
						   OnPropertyChanged(() => ResearchedAmount);
						   OnPropertyChanged(() => FiltersActivated);

						   _billingsCollectionView.Refresh();
					   }));
			}
		}

		public DelegateCommand ClearLabelResearchCommand
		{
			get
			{
				return _clearLabelResearchCommand ??
					   (_clearLabelResearchCommand = new DelegateCommand(() =>
																		 {
																			 ResearchedLabel = null;
																		 }));
			}
		}

		public DelegateCommand ClearAmountResearchCommand
		{
			get
			{
				return _clearAmountResearchCommand ??
					   (_clearAmountResearchCommand = new DelegateCommand(() =>
																		  {
																			  ResearchedAmount = null;
																		  }));
			}
		}

		public DelegateCommand StartResearchCommand
		{
			get
			{
				return _startResearchCommand ??
					   (_startResearchCommand = new DelegateCommand(() =>
																		  {
																			  IsExpanded = true;
																		  }));
			}
		}

		public DelegateCommand<object> CopyBillingsToClipboardCommand
		{
			get
			{
				return _copyBillingsToClipboardCommand ??
					   (_copyBillingsToClipboardCommand =
						   new DelegateCommand<object>(o =>
													   {
														   if (o == null)
															   return;

														   var collection = o as IList;

														   if (collection == null)
															   return;

														   var billings = collection
															   .OfType<BillingVom>()
															   .ToList();

														   var str = new StringBuilder();

														   str.Append("Numéro\t");
														   str.Append("Date de valeur\t");
														   str.Append("Date de transaction\t");
														   str.Append("Libellé\t");
														   str.Append("Montant\t");
														   str.Append("Différée\t");
														   str.Append("Pointée" + Environment.NewLine);

														   foreach (var billingVom in billings)
														   {
															   str.Append(billingVom.BillingId + "\t");
															   str.Append(billingVom.ValuationDate.ToString("dd/MM/yyyy") + "\t");
															   str.Append(billingVom.TransactionDate.ToString("dd/MM/yyyy") + "\t");
															   str.Append(billingVom.Title + "\t");
															   str.Append(
																   string.Format("{0}{1}",
																	   billingVom.Orientation == Orientation.Negative ? "-" : "",
																	   billingVom.Amount.ToString()) + "\t");
															   str.Append((billingVom.Delayed ? "1" : "0") + "\t");
															   str.Append((billingVom.Checked ? "1" : "0") + Environment.NewLine);
														   }

														   Clipboard.SetText(str.ToString(), TextDataFormat.Text);
													   }));
			}
		}

		public DelegateCommand<object> SelectionChangedCommand
		{
			get
			{
				return _selectionChangedCommand ??
					   (_selectionChangedCommand =
						   new DelegateCommand<object>(
							   o =>
							   {
								   if (o == null)
									   return;

								   var collection = o as IList;

								   if (collection == null)
									   return;

								   SelectedBillings = collection
									   .OfType<BillingVom>()
									   .ToList();
							   }));
			}
		}

		public DelegateCommand<object> CheckBillingCommand
		{
			get
			{
				return _checkBillingCommand ?? (_checkBillingCommand =
					new DelegateCommand<object>(paramObj =>
												{
													var billing = paramObj as BillingVom;

													if (billing == null)
														return;

													_businessContext
														.BillingManager
														.UpdateChecked(billing.BillingId, billing.Checked);
												}));
			}
		}

		public DelegateCommand<object> DelayBillingCommand
		{
			get
			{
				return _delayBillingCommand ?? (_delayBillingCommand =
					new DelegateCommand<object>(paramObj =>
													{
														var billing = paramObj as BillingVom;

														if (billing == null)
															return;

														_businessContext
															.BillingManager
															.UpdateDelayed(billing.BillingId, billing.Delayed);
													}));
			}
		}

		public DelegateCommand<object> EditBillingCommand
		{
			get
			{
				return _editBillingCommand ??
					   (_editBillingCommand =
						   new DelegateCommand<object>(paramObj =>
													   {
														   var billing = paramObj as BillingVom;

														   if (billing == null)
															   return;

														   NavigateToBillingEdition(billing.BillingId);
													   }));
			}
		}

		public DelegateCommand<object> DeleteBillingCommand
		{
			get
			{
				return _deleteBillingCommand ?? (_deleteBillingCommand =
					new DelegateCommand<object>(paramObj =>
					{
						if (paramObj == null)
							return;

						DeleteConfirmationRequest
							.Raise(new Confirmation
							{
								Title = "Confirmer ?",
								Content =
									"Confirmez-vous la suppression de cette ligne ?"
							}, confirmation =>
							{
								var billing = paramObj as BillingVom;

								if (billing == null)
									return;

								if (confirmation.Confirmed)
									_businessContext.BillingManager.RemoveBilling(
										billing.BillingId);
							});
					}));
			}
		}

		public DelegateCommand<object> MergeBillingsCommand
		{
			get
			{
				return _mergeBillingsCommand ?? (_mergeBillingsCommand =
					new DelegateCommand<object>(MergeBillingsCommandMethod));
			}
		}

		public DelegateCommand<object> ArchiveBillingCommand
		{
			get
			{
				return _archiveBillingCommand ?? (_archiveBillingCommand =
					new DelegateCommand<object>(ArchiveBillingCommandMethod));
			}
		}

		public InteractionRequest<IConfirmation> DeleteConfirmationRequest
		{
			get { return _deleteConfirmationRequest ?? (_deleteConfirmationRequest = new InteractionRequest<IConfirmation>()); }
		}

		private bool FilterBillings(object o)
		{
			var billing = o as BillingVom;

			if (billing == null)
				return false;

			if (!_showArchived && billing.IsArchived)
				return false;

			if (_showOnlySaving && !billing.IsSaving)
				return false;

			if (_transactionDateStart != null && billing.TransactionDate < _transactionDateStart.Value)
				return false;

			if (_transactionDateEnd != null && billing.TransactionDate > _transactionDateEnd.Value)
				return false;

			if (_showOnlyNotChecked && billing.Checked)
				return false;

			if (!string.IsNullOrEmpty(_researchedLabel)
				&& billing.Title.IndexOf(_researchedLabel, StringComparison.InvariantCultureIgnoreCase) == -1)
				return false;

			double amount;
			double delta;
			if (double.TryParse(_researchedAmount, out amount)
				&& double.TryParse(_deltaForResearchedAmount, out delta)
				&& (billing.Amount < amount - delta
					|| billing.Amount > amount + delta))
				return false;

			return true;
		}

		private void MergeBillingsCommandMethod(object paramObj)
		{
			if (!(paramObj is IList))
				return;

			var billings = ((IList)paramObj).OfType<BillingVom>().ToList();

			if (billings.Count <= 1)
				return;

			var newBillingId = _businessContext
				.BillingManager
				.MergeBillings(
					billings
						.Select(n => new BillingForMerging(
							n.BillingId,
							n.Amount,
							n.Orientation == Orientation.Positive,
							n.TransactionDate,
							n.ValuationDate,
							n.Checked,
							n.Delayed,
							n.Title))
						.ToList());

			NavigateToBillingEdition(newBillingId);
		}

		private void ArchiveBillingCommandMethod(object paramObj)
		{
			if (!(paramObj is IList))
				return;

			var billings = ((IList)paramObj).OfType<BillingVom>().ToList();

			if (billings.Count == 0)
				return;

			foreach (var b in billings)
			{
				b.IsArchived = !b.IsArchived;

				_businessContext
					.BillingManager
					.UpdateArchived(b.BillingId, b.IsArchived);
			}

			_billingsCollectionView.Refresh();
		}


		private void NavigateToBillingEdition(int billingId)
		{
			var parameter = new NavigationParameters();

			parameter.Add("BillingId", billingId);

			_regionManager.RequestNavigate(
				RegionNames.LeftDockRegion,
				ViewNames.EditingView, parameter);
		}

		#region ITab

		public string HeaderTab
		{
			get { return "Listing"; }
		}

		public DelegateCommand CloseTabCommand
		{
			get { return _closeTabCommand ?? (_closeTabCommand = new DelegateCommand(() => Close())); }
		}

		#endregion

		#region IClosable

		public Action Close { get; set; }

		#endregion

		#region ISpecificCommands

		private List<SimpleCommand> _commands;

		public IEnumerable<SimpleCommand> Commands
		{
			get
			{
				return _commands ??
					   (_commands = new List<SimpleCommand>
									{
										new SimpleCommand
										{
											Label = "Modifier",
											Command =
												new DelegateCommand(() => EditBillingCommand.Execute(_billingsCollectionView.CurrentItem),
												() => _billingsCollectionView.CurrentItem != null),
											IconResourceName = "EditSource",
										},

										new SimpleCommand
										{
											Label = "Fusionner",
											Command = new DelegateCommand(() => MergeBillingsCommand.Execute(SelectedBillings),
												() => SelectedBillings != null && SelectedBillings.Count > 1),
											IconResourceName = "MergeSource",
										},

										new SimpleCommand
										{
											Label = "Archiver",
											Command =
												new DelegateCommand(() => ArchiveBillingCommand.Execute(SelectedBillings),
												() => SelectedBillings != null && SelectedBillings.Count >= 1),
											IconResourceName = "ArchiveImageSource",
										},

										new SimpleCommand
										{
											Label = "Supprimer",
											Command =
												new DelegateCommand(() => DeleteBillingCommand.Execute(_billingsCollectionView.CurrentItem),
												() => _billingsCollectionView.CurrentItem != null),
											IconResourceName = "TrashSource",
										},
									});
			}
		}

		#endregion

		#region IViewRemovedAware

		public void OnViewRemovedFromRegion()
		{
			foreach (var subscriptionToken in _subscriptionTokens.ToArray())
			{
				subscriptionToken.Dispose();
				_subscriptionTokens.Remove(subscriptionToken);
			}
		}

		#endregion

	}

	public class BillingVom : BindableBase
	{
		private bool _checked;
		private bool _delayed;
		private bool _isArchived;

		public BillingVom(Billing billing)
		{
			BillingId = billing.Id;
			IsArchived = billing.IsArchived;
			IsSaving = billing.IsSaving;
			TransactionDate = billing.TransactionDate;
			ValuationDate = billing.ValuationDate;
			Title = billing.Title;
			Amount = billing.Amount;
			Orientation = OrientationConverter.ConvertToEnum(billing.Positive);
			Checked = billing.Checked;
			Delayed = billing.Delayed;
			Comment = billing.Comment;
			BillingId = billing.Id;
		}

		public int BillingId { get; }
		public DateTime TransactionDate { get; }
		public DateTime ValuationDate { get; }
		public string Title { get; }
		public double Amount { get; }
		public Orientation Orientation { get; }

		public bool Checked
		{
			get { return _checked; }
			set { SetProperty(ref _checked, value); }
		}
		public bool Delayed
		{
			get { return _delayed; }
			set { SetProperty(ref _delayed, value); }
		}
		public bool IsArchived
		{
			get { return _isArchived; }
			set { SetProperty(ref _isArchived, value); }
		}
		public bool IsSaving { get; }
		public string Comment { get; private set; }

		public bool ShowComment
		{
			get { return !string.IsNullOrWhiteSpace(Comment); }
		}
	}

}
