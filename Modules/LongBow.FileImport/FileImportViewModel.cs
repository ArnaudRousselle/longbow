using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using LongBow.Common.Enumerations;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.Interfaces.File;
using LongBow.Common.Interfaces.Tabulation;
using LongBow.Common.Regions;
using LongBow.Dom;
using LongBow.FileImport.Enums;
using LongBow.FileImport.Interfaces;
using LongBow.FileImport.Objects;
using LongBow.FileImport.Services.BankPluginSelectionService;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Mvvm;
using System.Collections.ObjectModel;
using LongBow.FileImport.SearchBilling;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.FileImport
{
	[Export(typeof(IFileImportViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class FileImportViewModel : BindableBase, IFileImportViewModel, IClosable, ITab, ISpecificCommands, INavigationAware
	{
		~FileImportViewModel()
		{
			_loggerFacade.Log("FileImportViewModel garbage collected", Category.Debug, Priority.Low);
		}

		private readonly IFileOpenDialogService _fileOpenDialogService;
		private readonly IFileReaderFactory _fileReaderFactory;
		private readonly IBusinessContext _businessContext;
		private readonly ILoggerFacade _loggerFacade;

		private InteractionRequest<ISearchBillingConfirmation> _searchBillingRequest;
		private string _bankId;
		private string _branchId;
		private string _accountId;
		private string _currency;
		private bool _delayed;
		private DateTime? _valuationDate;
		private TitleField _titleField = TitleField.Label;
		private DelegateCommand _chooseFileCommand;
		private DelegateCommand _refreshCommand;
		private DelegateCommand _closeTabCommand;
		private DelegateCommand<DataLineVom> _executeActionForDataLineCommand;

		[ImportingConstructor]
		public FileImportViewModel(IFileOpenDialogService fileOpenDialogService,
			IFileReaderFactory fileReaderFactory,
			IBusinessContext businessContext,
			ILoggerFacade loggerFacade)
		{
			_fileOpenDialogService = fileOpenDialogService;
			_fileReaderFactory = fileReaderFactory;
			_businessContext = businessContext;
			_loggerFacade = loggerFacade;

			DataLines = new ObservableCollection<DataLineVom>();

			BankPluginSelectionRequest = new InteractionRequest<BankPluginSelectionNotification>();
			NotificationRequest = new InteractionRequest<Notification>();
		}

		#region IClosable

		public Action Close { get; set; }

		#endregion

		#region ITab

		public string HeaderTab
		{
			get { return "Importation"; }
		}

		public DelegateCommand CloseTabCommand
		{
			get { return _closeTabCommand ?? (_closeTabCommand = new DelegateCommand(() => Close())); }
		}

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
											Label = "Ouvrir",
											Command =
												new DelegateCommand(() => ChooseFileCommand.Execute()),
											IconResourceName = "OpenSmallSource",
										},

										new SimpleCommand
										{
											Label = "Recharger",
											Command = new DelegateCommand(() => RefreshCommand.Execute()),
											IconResourceName = "RefreshSource",
										},
									});
			}
		}

		#endregion

		public InteractionRequest<ISearchBillingConfirmation> SearchBillingRequest =>
			_searchBillingRequest ?? (_searchBillingRequest = new InteractionRequest<ISearchBillingConfirmation>());

		public string BankId
		{
			get { return _bankId; }
			set { SetProperty(ref _bankId, value); }
		}

		public string BranchId
		{
			get { return _branchId; }
			set { SetProperty(ref _branchId, value); }
		}

		public string AccountId
		{
			get { return _accountId; }
			set { SetProperty(ref _accountId, value); }
		}

		public string Currency
		{
			get { return _currency; }
			set { SetProperty(ref _currency, value); }
		}

		public bool Delayed
		{
			get { return _delayed; }
			set
			{
				SetProperty(ref _delayed, value);
				ValuationDate = !value
					? default(DateTime?)
					: new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
			}
		}

		public DateTime? ValuationDate
		{
			get { return _valuationDate; }
			set { SetProperty(ref _valuationDate, value); }
		}

		public TitleField TitleField
		{
			get { return _titleField; }
			set { SetProperty(ref _titleField, value); }
		}

		public ObservableCollection<DataLineVom> DataLines { get; private set; }

		public DelegateCommand ChooseFileCommand
		{
			get
			{
				return _chooseFileCommand ??
					   (_chooseFileCommand = new DelegateCommand(() =>
						   _fileOpenDialogService.OpenFile(
							   "Fichiers compatibles (*.csv, *.ofx)|*.csv;*.ofx" +
							   "|Fichiers CSV (*.csv)|*.csv" +
							   "|Fichiers OFX (*.ofx)|*.ofx",
							   ChooseFileCommandCallBack)));
			}
		}

		public DelegateCommand RefreshCommand
		{
			get
			{
				return _refreshCommand ??
					   (_refreshCommand = new DelegateCommand(RefreshCommandMethod));
			}
		}

		public DelegateCommand<DataLineVom> ExecuteActionForDataLineCommand
		{
			get
			{
				return _executeActionForDataLineCommand ??
					 (_executeActionForDataLineCommand = new DelegateCommand<DataLineVom>(ExecuteActionForDataLineCommandMethod));
			}
		}

		public InteractionRequest<BankPluginSelectionNotification> BankPluginSelectionRequest { get; private set; }
		public InteractionRequest<Notification> NotificationRequest { get; private set; }

		private void ChooseFileCommandCallBack(FileOpenDialogServiceResult result)
		{
			if (!result.Confirmed)
				return;

			var availableBankPlugins = _fileReaderFactory.GetAvailableBankPlugins(result.FileName);

			if (availableBankPlugins.Count == 1)
				ReadFileAndPrintData(result.FileName, availableBankPlugins[0]);
			else
			{
				BankPluginSelectionRequest
					.Raise(
						new BankPluginSelectionNotification(availableBankPlugins)
						{
							Title = "Choix du plugin",
						},
						notification =>
						{
							if (!notification.Confirmed || notification.SelectedBankPlugin == null)
								return;

							ReadFileAndPrintData(result.FileName, notification.SelectedBankPlugin.Value);
						});
			}
		}

		private void ReadFileAndPrintData(string fileName, BankPlugin bankPlugin)
		{
			string error;

			var data = _fileReaderFactory.ReadFile(fileName, out error, bankPlugin);

			if (error != null)
			{
				NotificationRequest.Raise(new Notification
				{
					Title = "Erreur",
					Content = error,
				});
				return;
			}

			BankId = data.BankId;
			BranchId = data.BranchId;
			AccountId = data.AcctId;
			Currency = data.CurDef;

			DataLines.Clear();
			data.Lines.ForEach(n => DataLines.Add(new DataLineVom(n)));

			RefreshCommandMethod();
		}

		private void RefreshCommandMethod()
		{
			foreach (var dataLine in DataLines)
			{
				List<Billing> perfectMatchings;
				List<Billing> incompleteMatchings;

				_businessContext
					.BillingManager
					.FindBilling(_delayed ? (_valuationDate ?? DateTime.Now) : dataLine.Date,
						dataLine.Amount,
						out perfectMatchings,
						out incompleteMatchings);

				dataLine.Matchings.Clear();

				perfectMatchings.ForEach(
					n => dataLine.Matchings.Add(new Matching(n, true)));

				incompleteMatchings.ForEach(
					n => dataLine.Matchings.Add(new Matching(n, false)));

				dataLine.Matchings.Add(Matching.CreateNewItem());
				dataLine.Matchings.Add(Matching.CreateOtherItem());

				dataLine.SelectedMatching = dataLine.Matchings[0];
			}
		}

		private void ExecuteActionForDataLineCommandMethod(DataLineVom dataLineVom)
		{
			var selectedMatching = dataLineVom.SelectedMatching;

			if (selectedMatching.NewItem)
			{
				// l'opération est inexistante, il faut la créer

				string title;

				switch(_titleField)
				{
					case FileImport.TitleField.Label:
						title = dataLineVom.Name;
						break;

					case FileImport.TitleField.Memo:
						title = dataLineVom.Memo;
						break;

					default:
						title = "";
						break;
				}
				
				var billing = new Billing
				{
					TransactionDate = DateTime.Now,
					ValuationDate = _valuationDate ?? dataLineVom.Date,
					Title = title,
					Amount = Math.Abs(dataLineVom.Amount),
					Positive = dataLineVom.Amount >= 0,
					Checked = true,
					Delayed = _delayed,
					Comment = "créée automatiquement par les données reçues de la banque",
				};

				_businessContext.BillingManager.AddBilling(billing);

				dataLineVom.Matchings.Insert(0, new Matching(billing, true));
				dataLineVom.SelectedMatching = dataLineVom.Matchings[0];

				return;
			}

			if (selectedMatching.OtherItem)
			{
				SearchBillingRequest.Raise(new SearchBillingConfirmation(),
					confirmation =>
					{
						if (!confirmation.Confirmed)
							return;

						confirmation.Billing.Checked = true;
						confirmation.Billing.Amount = Math.Abs(dataLineVom.Amount);
						confirmation.Billing.Positive = dataLineVom.Amount >= 0;
						confirmation.Billing.ValuationDate = dataLineVom.Date;

						_businessContext
							.BillingManager
							.UpdateBilling(confirmation.Billing);

						dataLineVom.Matchings.Insert(0, new Matching(confirmation.Billing, true));
						dataLineVom.SelectedMatching = dataLineVom.Matchings[0];
					});

				return;
			}

			if (!selectedMatching.Checked)
			{
				// l'opération existe, mais n'est pas pointée
				// il suffit de pointer l'opération
				dataLineVom.SelectedMatching.Checked = true;
				_businessContext
					.BillingManager
					.UpdateChecked(selectedMatching.BillingId, true);
			}

		}

		#region INavigationAware

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			ChooseFileCommand.Execute();
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}

		#endregion
	}

	public class DataLineVom : BindableBase
	{
		private Matching _selectedMatching;

		public DataLineVom(DataLine line)
		{
			AccountId = line.AccountId;
			Date = line.DtPosted;
			OperationNumber = line.FitId;
			Name = line.Name;
			Memo = line.Memo;
			Amount = line.TrnAmt;
			Matchings = new ObservableCollection<Matching>();
		}

		public DateTime Date { get; set; }

		public string OperationNumber { get; set; }

		public string Name { get; set; }

		public string Memo { get; set; }

		public double Amount { get; set; }

		public string AccountId { get; set; }

		public ObservableCollection<Matching> Matchings { get; private set; }

		public Matching SelectedMatching
		{
			get { return _selectedMatching; }
			set { SetProperty(ref _selectedMatching, value); }
		}
	}

	public class Matching : BindableBase
	{
		private bool _checked;

		public int BillingId { get; private set; }
		public string Label { get; private set; }
		public bool NewItem { get; private set; }
		public bool OtherItem { get; private set; }

		public bool Checked
		{
			get { return _checked; }
			set { SetProperty(ref _checked, value); }
		}

		public bool CompleteMatching { get; private set; }

		public Matching(Billing billing, bool completeMatching)
		{
			BillingId = billing.Id;

			Label = string.Format("{0} - {1} ({2:dd/MM/yyyy})",
				billing.Id,
				billing.Title,
				billing.ValuationDate);

			Checked = billing.Checked;

			CompleteMatching = completeMatching;

			NewItem = false;
			OtherItem = false;
		}

		public static Matching CreateNewItem()
		{
			return new Matching
			{
				NewItem = true,
				OtherItem = false,
				Label = "<Nouvelle>"
			};
		}

		public static Matching CreateOtherItem()
		{
			return new Matching
			{
				NewItem = false,
				OtherItem = true,
				Label = "<Autre>"
			};
		}

		private Matching()
		{
		}
	}
}
