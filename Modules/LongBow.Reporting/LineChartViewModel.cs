using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.Interfaces.Tabulation;
using LongBow.Common.Regions;
using LongBow.Dom;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.Reporting
{
	//todo: tracer des histogrammes par mois et par mode de paiement
	//todo: mettre en forme le texte dans la popup qui s'affiche au survol du graphique (mettre le montant et le symbole €)
	[Export(typeof(ILineChartViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class LineChartViewModel : BindableBase, ILineChartViewModel, ITab, IClosable, ISpecificCommands, INavigationAware
	{
		private readonly IBusinessContext _businessContext;

		private bool _allBillings = false;
		private DateTime _startDate = DateTime.Now.Date.AddMonths(-6);
		private DateTime _endDate = DateTime.Now.Date.AddMonths(2);
		private bool _chartVisible;
		private DelegateCommand _closeTabCommand;
		private IEnumerable<SimpleCommand> _commands;

	    [ImportingConstructor]
		public LineChartViewModel(IBusinessContext businessContext)
		{
			_businessContext = businessContext;
            ChartItems = new List<PointItem>();
        }

		private void Draw()
		{
			#region Values

			var billings = _businessContext
				.BillingManager
				.GetBillings();

			#region Future values

			var maxFutureDate = DateTime.Now.AddYears(1);

			var repetitiveBillings = _businessContext
				.RepetitiveBillingManager
				.GetRepetitiveBillings();

			foreach (var rb in repetitiveBillings)
			{
				while (rb.ValuationDate < maxFutureDate)
				{
					billings.Add(new Billing
					{
						ValuationDate = rb.ValuationDate,
						Amount = rb.Amount,
						Positive = rb.Positive
					});

					rb.ShiftValuationDate();
				}
			}

			#endregion

			DateTime date;
			DateTime maxDate;

			if (_allBillings)
			{
				if (billings.Any())
				{
					date = billings.Min(n => n.ValuationDate);
					maxDate = billings.Max(n => n.ValuationDate).Date.AddHours(12);
				}
				else
				{
					date = DateTime.Now;
					maxDate = DateTime.Now.Date.AddHours(12);
				}
			}
			else
			{
				date = _startDate.Date;
				maxDate = _endDate.Date.AddHours(12);
			}

			ChartItems.Clear();

			double value;

			while (date < maxDate)
			{
				value = billings
					.Where(n => n.ValuationDate <= date)
					.Sum(n => n.Amount * (n.Positive ? 1 : -1));

				ChartItems.Add(new PointItem
				{
					Date = date,
					Value = value
				});

				date = date.AddDays(1);
			}

			#endregion

			RefreshChart();

            ChartVisible = true;
		}

		#region ILineChartViewModel

		public DateTime StartDate
		{
			get { return _startDate; }
			set { SetProperty(ref _startDate, value); }
		}

		public DateTime EndDate
		{
			get { return _endDate; }
			set { SetProperty(ref _endDate, value); }
		}

		public bool AllBillings
		{
			get { return _allBillings; }
			set { SetProperty(ref _allBillings, value); }
		}

		public bool ChartVisible
		{
			get { return _chartVisible; }
			private set { SetProperty(ref _chartVisible, value); }
		}

	    public List<PointItem> ChartItems { get; }

		public Action RefreshChart { get; set; }

		#endregion

		#region ITab

		public string HeaderTab
		{
			get { return "Graphique"; }
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

		public IEnumerable<SimpleCommand> Commands
		{
			get
			{
				return _commands
				       ?? (_commands = new List<SimpleCommand>
				                       {
					                       new SimpleCommand
					                       {
						                       Command = new DelegateCommand(Draw),
						                       IconResourceName = "DrawSource",
						                       Label = "Tracer"
					                       }
				                       });
			}
		}

		#endregion

		#region INavigationAware

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			Draw();
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

    public class PointItem
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }
}
