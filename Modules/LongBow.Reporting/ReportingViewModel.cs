using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LongBow.Common.EventMessages;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.Interfaces.Tabulation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;

namespace LongBow.Reporting
{
	[Export(typeof (IReportingViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class ReportingViewModel : BindableBase, IReportingViewModel, ITab, IClosable
	{
		~ReportingViewModel()
		{
			_loggerFacade.Log("ReportingViewModel garbage collected", Category.Debug, Priority.Low);
		}

		private readonly IBusinessContext _businessContext;
		private readonly ILoggerFacade _loggerFacade;

        private double? _currentTotalBalance;
		private double? _monthBalance;
		private double? _checkedBalance;
		private DateTime _referenceDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
		private DelegateCommand _closeTabCommand;
	    private List<DelayedSubTotal> _totalDelayedItems;
	    private List<DelayedSubTotal> _checkedDelayedItems;

	    [ImportingConstructor]
		public ReportingViewModel(IEventAggregator eventAggregator,
			IBusinessContext businessContext,
			ILoggerFacade loggerFacade)
		{
			_businessContext = businessContext;
			_loggerFacade = loggerFacade;

			eventAggregator
				.GetEvent<AddedBillingEvent>()
				.Subscribe(args => CalculateAsync());
			
			eventAggregator
				.GetEvent<DeletedBillingEvent>()
				.Subscribe(args => CalculateAsync());

			eventAggregator
				.GetEvent<UpdatedBillingEvent>()
				.Subscribe(args => CalculateAsync());

			eventAggregator
				.GetEvent<UpdateCheckedBillingEvent>()
				.Subscribe(args => CalculateAsync());

			eventAggregator
				.GetEvent<UpdateDelayedBillingEvent>()
				.Subscribe(args => CalculateAsync());

			CalculateAsync();
		}

	    public double? CurrentTotalBalance
		{
			get { return _currentTotalBalance; }
			set { SetProperty(ref _currentTotalBalance, value); }
		}

		public double? CheckedBalance
		{
			get { return _checkedBalance; }
			set { SetProperty(ref _checkedBalance, value); }
		}

	    public List<DelayedSubTotal> TotalDelayedItems
	    {
	        get { return _totalDelayedItems; }
	        private set { SetProperty(ref _totalDelayedItems, value); }
	    }

	    public List<DelayedSubTotal> CheckedDelayedItems
	    {
	        get { return _checkedDelayedItems; }
            private set { SetProperty(ref _checkedDelayedItems, value); }
        }

		private async Task CalculateAsync()
		{
			await Task
				.Factory
				.StartNew(Calculate);
		}

		private void Calculate()
		{
			var allBillings = _businessContext.BillingManager.GetBillings();
			var result = _businessContext.BillingCalculationManager.Calculate(allBillings);

			CurrentTotalBalance = result.CurrentTotalBalance;
			CheckedBalance = result.CheckedBalance;
            TotalDelayedItems = result.TotalDelayedItems;
            CheckedDelayedItems = result.CheckedDelayedItems;
        }

		#region ITab

		public string HeaderTab
		{
			get { return "Reporting"; }
		}

		public DelegateCommand CloseTabCommand
		{
			get { return _closeTabCommand ?? (_closeTabCommand = new DelegateCommand(() => Close())); }
		}

		#endregion

		#region IClosable

		public Action Close { get; set; } 

		#endregion

	}
}
