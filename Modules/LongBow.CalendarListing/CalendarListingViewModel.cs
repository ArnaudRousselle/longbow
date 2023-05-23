using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using LongBow.Common.Contracts;
using LongBow.Common.Enumerations;
using LongBow.Common.EventMessages;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.Interfaces.Tabulation;
using LongBow.Common.Regions;
using LongBow.Dom;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.CalendarListing
{
    [Export(typeof(ICalendarListingViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CalendarListingViewModel : BindableBase, ICalendarListingViewModel, IClosable, ITab, ISpecificCommands
    {
        ~CalendarListingViewModel()
        {
            _loggerFacade.Log("CalendarListingViewModel garbage collected", Category.Debug, Priority.Low);
        }

        private readonly IBusinessContext _businessContext;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _loggerFacade;

        private readonly ICollectionView _repetitiveBillingsCollectionView;

        private double _averageMonthlyBalance;
        private DelegateCommand<object> _createBillingCommand;
        private DelegateCommand<IList> _createMultipleBillingCommand;
        private DelegateCommand _createRepetitiveBillingCommand;
        private DelegateCommand<object> _editRepetitiveBillingCommand;
        private DelegateCommand<object> _deleteRepetitiveBillingCommand;
        private InteractionRequest<IConfirmation> _deleteConfirmationRequest;
        private DelegateCommand _closeTabCommand;

        [ImportingConstructor]
        public CalendarListingViewModel(IBusinessContext businessContext,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            ILoggerFacade loggerFacade)
        {
            _businessContext = businessContext;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _loggerFacade = loggerFacade;

            RepetitiveBillings =
                new ObservableCollection<RepetitiveBillingVom>(
                    _businessContext
                        .RepetitiveBillingManager
                        .GetRepetitiveBillings()
                        .Select(n => new RepetitiveBillingVom(n)));

            _repetitiveBillingsCollectionView = CollectionViewSource.GetDefaultView(RepetitiveBillings);
            _repetitiveBillingsCollectionView.CurrentChanged += RepetitiveBillingsCollectionViewCurrentChanged;

            _repetitiveBillingsCollectionView.SortDescriptions.Add(new SortDescription("ValuationDate", ListSortDirection.Ascending));

            _eventAggregator
                .GetEvent<AddedRepetitiveBillingEvent>()
                .Subscribe(args =>
                           {
                               RepetitiveBillings.Add(new RepetitiveBillingVom(args.AddedRepetitiveBilling));
                               CalculateAsync();
                           });

            _eventAggregator
                .GetEvent<UpdatedRepetitiveBillingEvent>()
                .Subscribe(args =>
                           {
                               var billingToUpdate = RepetitiveBillings
                                   .FirstOrDefault(n => n.RepetitiveBillingId == args.UpdatedRepetitiveBilling.Id);

                               if (billingToUpdate == null)
                                   return;

                               var index = RepetitiveBillings.IndexOf(billingToUpdate);

                               RepetitiveBillings.RemoveAt(index);
                               RepetitiveBillings.Insert(index, new RepetitiveBillingVom(args.UpdatedRepetitiveBilling));

                               CalculateAsync();
                           });

            _eventAggregator
                .GetEvent<DeletedRepetitiveBillingEvent>()
                .Subscribe(args =>
                           {
                               var billing = RepetitiveBillings.FirstOrDefault(n => n.RepetitiveBillingId == args.RepetitiveBillingId);

                               if (billing != null)
                                   RepetitiveBillings.Remove(billing);

                               CalculateAsync();
                           });

            CalculateAsync();
        }

        public double AverageMonthlyBalance
        {
            get { return _averageMonthlyBalance; }
            private set { SetProperty(ref _averageMonthlyBalance, value); }
        }

        public ObservableCollection<RepetitiveBillingVom> RepetitiveBillings { get; private set; }

        public DelegateCommand<object> CreateBillingCommand
        {
            get
            {
                return _createBillingCommand ??
                       (_createBillingCommand = new DelegateCommand<object>(
                           CreateBillingCommandMethod));
            }
        }

        public DelegateCommand<IList> CreateMultipleBillingCommand
        {
            get
            {
                return _createMultipleBillingCommand ??
                       (_createMultipleBillingCommand = new DelegateCommand<IList>(
                           list =>
                           {
                               if (list.Count == 0)
                                   return;

                               if (list.Count == 1)
                               {
                                   CreateBillingCommandMethod(list[0]);
                                   return;
                               }

                               foreach (var repetitiveBillingVom in list
                                   .OfType<RepetitiveBillingVom>()
                                   .ToArray())
                               {
                                   var parameter = new NavigationParameters();

                                   parameter.Add("RepetitiveBillingId", repetitiveBillingVom.RepetitiveBillingId);

                                   parameter.Add("AutoInsert", true);

                                   _regionManager.RequestNavigate
                                       (RegionNames.NewWindowRegion, ViewNames.EditingView, parameter);
                               }
                           }));
            }
        }

        public DelegateCommand CreateRepetitiveBillingCommand
        {
            get
            {
                return _createRepetitiveBillingCommand ??
                       (_createRepetitiveBillingCommand = new DelegateCommand(() => _regionManager.RequestNavigate
                           (RegionNames.NewWindowRegion,
                               ViewNames.RepetitiveBillingCreationView)));
            }
        }

        public DelegateCommand<object> EditRepetitiveBillingCommand
        {
            get
            {
                return _editRepetitiveBillingCommand ??
                       (_editRepetitiveBillingCommand = new DelegateCommand<object>(
                           paramObj =>
                           {
                               var repetitiveBillingVom = paramObj as RepetitiveBillingVom;

                               if (repetitiveBillingVom == null)
                                   return;

                               var parameter = new NavigationParameters();

                               parameter.Add("RepetitiveBillingId",
                                   repetitiveBillingVom.RepetitiveBillingId);

                               _regionManager.RequestNavigate
                                   (
                                       RegionNames.NewWindowRegion,
                                       ViewNames.RepetitiveBillingCreationView, parameter);
                           }));
            }
        }

        public DelegateCommand<object> DeleteRepetitiveBillingCommand
        {
            get
            {
                return _deleteRepetitiveBillingCommand ??
                       (_deleteRepetitiveBillingCommand = new DelegateCommand<object>(
                           paramObj =>
                           {
                               if (paramObj == null)
                                   return;

                               DeleteConfirmationRequest
                                   .Raise(new Confirmation
                                          {
                                              Title = "Confirmer ?",
                                              Content =
                                                  "Confirmez-vous la suppression de cette ligne ?",
                                          },
                                       confirmation =>
                                       {
                                           var repetitiveBillingVom = paramObj as RepetitiveBillingVom;

                                           if (repetitiveBillingVom == null)
                                               return;

                                           if (confirmation.Confirmed)
                                               _businessContext
                                                   .RepetitiveBillingManager
                                                   .RemoveRepetitiveBilling(repetitiveBillingVom.RepetitiveBillingId);
                                       });
                           } ));
        }
        }

        public InteractionRequest<IConfirmation> DeleteConfirmationRequest
        {
            get { return _deleteConfirmationRequest ?? (_deleteConfirmationRequest = new InteractionRequest<IConfirmation>()); }
        }

        private void RepetitiveBillingsCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            foreach (var simpleCommand in Commands)
            {
                simpleCommand.Command.RaiseCanExecuteChanged();
            }
        }

        private async Task CalculateAsync()
        {
            await Task
                .Factory
                .StartNew(Calculate);
        }

        private void Calculate()
        {
            var allRepetitiveBillings = _businessContext.RepetitiveBillingManager.GetRepetitiveBillings();
            var result = _businessContext.BillingCalculationManager.Calculate(allRepetitiveBillings);

            AverageMonthlyBalance = result.AverageMonthlyBalance;
        }

        private void CreateBillingCommandMethod(object arg)
        {
            var repetitiveBillingVom = arg as RepetitiveBillingVom;

            if (repetitiveBillingVom == null)
                return;

            var parameter = new NavigationParameters();

            parameter.Add("RepetitiveBillingId",
                repetitiveBillingVom.RepetitiveBillingId);

            _regionManager.RequestNavigate
                (
                    RegionNames.LeftDockRegion,
                    ViewNames.EditingView, parameter);
        }

        #region IClosable

        public Action Close { get; set; }

        #endregion

        #region ITab

        public string HeaderTab
        {
            get { return "Echéancier"; }
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
                                            Label = "Nouveau",
                                            Command = CreateRepetitiveBillingCommand,
                                            IconResourceName = "AddSmallSource",
                                        },

                                        new SimpleCommand
                                        {
                                            Label = "Insérer",
                                            Command = new DelegateCommand(() => CreateBillingCommand.Execute(_repetitiveBillingsCollectionView.CurrentItem as RepetitiveBillingVom),
                                                () => _repetitiveBillingsCollectionView.CurrentItem != null),
                                            IconResourceName = "InsertSource",
                                        },

                                        new SimpleCommand
                                        {
                                            Label = "Modifier",
                                            Command =
                                                new DelegateCommand(() => EditRepetitiveBillingCommand.Execute(_repetitiveBillingsCollectionView.CurrentItem),
                                                () => _repetitiveBillingsCollectionView.CurrentItem != null),
                                            IconResourceName = "EditSource",
                                        },

                                        new SimpleCommand
                                        {
                                            Label = "Supprimer",
                                            Command =
                                                new DelegateCommand(() => DeleteRepetitiveBillingCommand.Execute(_repetitiveBillingsCollectionView.CurrentItem),
                                                () => _repetitiveBillingsCollectionView.CurrentItem != null),
                                            IconResourceName = "TrashSource",
                                        },
                                    });
            }
        }

        #endregion

    }

    public class RepetitiveBillingVom
    {
        public RepetitiveBillingVom(RepetitiveBilling repetitiveBilling)
        {
            RepetitiveBillingId = repetitiveBilling.Id;
            ValuationDate = repetitiveBilling.ValuationDate;
            Title = repetitiveBilling.Title;
            Amount = repetitiveBilling.Amount;
            Orientation = OrientationConverter.ConvertToEnum(repetitiveBilling.Positive);
            FrequenceMode = FrequenceModeConverter.ConvertToEnum(repetitiveBilling.FrequenceMode);

            var comparaisonDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
            ValuationDateInCurrentMonth = ValuationDate < comparaisonDate;
        }

        public int RepetitiveBillingId { get; set; }
        public DateTime ValuationDate { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }
        public Orientation Orientation { get; set; }
        public FrequenceMode FrequenceMode { get; set; }
        public bool ValuationDateInCurrentMonth { get; set; }
    }
}
