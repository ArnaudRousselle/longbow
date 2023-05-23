using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;
using LongBow.Common.Enumerations;
using LongBow.Common.EventMessages;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.Interfaces.Tabulation;
using LongBow.Dom;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.RepetitiveBillingCreation
{
	[Export(typeof(IRepetitiveBillingCreationViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class RepetitiveBillingCreationViewModel : BindableBase, IRepetitiveBillingCreationViewModel,
		INotifyDataErrorInfo, IClosable, INavigationAware, ITab
	{
		~RepetitiveBillingCreationViewModel()
		{
			_loggerFacade.Log("RepetitiveBillingCreationViewModel garbage collected " + _editingRepetitiveBillingId, Category.Debug, Priority.Low);
		}

		private readonly IBusinessContext _businessContext;
		private readonly IEventAggregator _eventAggregator;
		private readonly ILoggerFacade _loggerFacade;

		private int _editingRepetitiveBillingId;
		private DateTime _valuationDate;
		private string _title;
		private string _amount;
		private Orientation _orientation;
		private FrequenceMode _frequenceMode;
		private DelegateCommand _validateCommand;

		[ImportingConstructor]
		public RepetitiveBillingCreationViewModel(IBusinessContext businessContext,
			IEventAggregator eventAggregator,
			ILoggerFacade loggerFacade)
		{
			_businessContext = businessContext;
			_eventAggregator = eventAggregator;
			_loggerFacade = loggerFacade;
		}

		public DateTime ValuationDate
		{
			get { return _valuationDate; }
			set { SetProperty(ref _valuationDate, value); }
		}

		public string Title
		{
			get { return _title; }
			set
			{
				ClearErrors(() => Title);

				if (string.IsNullOrWhiteSpace(value))
					SetErrors(() => Title, "Vous devez renseigner un titre");

				SetProperty(ref _title, value);
			}
		}

		public string Amount
		{
			get { return _amount; }
			set
			{
				ClearErrors(() => Amount);

				if (string.IsNullOrWhiteSpace(value))
					SetErrors(() => Amount, "Vous devez renseigner un montant");

				double d;
				if (!double.TryParse(value, out d))
					SetErrors(() => Amount, "Le montant doit être un nombre réel");

				SetProperty(ref _amount, value);
			}
		}

		public Orientation Orientation
		{
			get { return _orientation; }
			set { SetProperty(ref _orientation, value); }
		}

		public FrequenceMode FrequenceMode
		{
			get { return _frequenceMode; }
			set { SetProperty(ref _frequenceMode, value); }
		}

		public DelegateCommand ValidateCommand
		{
			get
			{
				return _validateCommand ?? (_validateCommand = new DelegateCommand(ValidateCommandMethod));
			}
		}

		private void ValidateCommandMethod()
		{
			RepetitiveBilling repetitiveBilling;

			#region mapping

			repetitiveBilling = new RepetitiveBilling
			{
				ValuationDate = ValuationDate,
				Title = Title,
				Amount = double.Parse(Amount),
				Positive = OrientationConverter.ConvertToBool(Orientation),
				FrequenceMode = FrequenceModeConverter.ConvertToInt(FrequenceMode),
			};

			#endregion

			if (_editingRepetitiveBillingId == 0)
			{
				_businessContext.RepetitiveBillingManager.AddRepetitiveBilling(repetitiveBilling);
			}
			else
			{
				repetitiveBilling.Id = _editingRepetitiveBillingId;

				_businessContext.RepetitiveBillingManager.UpdateRepetitiveBilling(repetitiveBilling);
			}

			Close();
		}

		#region INotifyDataErrorInfo

		private readonly Dictionary<string, string> _errorDictionary
			= new Dictionary<string, string>();

		public IEnumerable GetErrors(string propertyName)
		{
			return _errorDictionary.ContainsKey(propertyName)
				? new List<string> {_errorDictionary[propertyName]}
				: null;
		}

		public bool HasErrors
		{
			get { return _errorDictionary.Any(); }
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		protected void RaiseErrorsChanged(string propertyName)
		{
			var handler = ErrorsChanged;

			if (handler == null)
				return;

			handler(this, new DataErrorsChangedEventArgs(propertyName));
		}

		private void ClearErrors<TProperty>(Expression<Func<TProperty>> propertyExpression)
		{
			var propertyName = GetPropertyName(propertyExpression);
			_errorDictionary.Remove(propertyName);
			RaiseErrorsChanged(propertyName);
			ValidateCommand.RaiseCanExecuteChanged();
		}

		private void SetErrors<TProperty>(Expression<Func<TProperty>> propertyExpression, string error)
		{
			var propertyName = GetPropertyName(propertyExpression);
			_errorDictionary[propertyName] = error;
			RaiseErrorsChanged(propertyName);
			ValidateCommand.RaiseCanExecuteChanged();
		}

		private string GetPropertyName<TProperty>(Expression<Func<TProperty>> propertyExpression)
		{
			var memberExpression = propertyExpression.Body as MemberExpression;

			return memberExpression != null ? memberExpression.Member.Name : string.Empty;
		}

		#endregion

		#region IClosable

		public Action Close { get; set; }

		#endregion

		#region INavigationAware

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			var parameter = navigationContext.Parameters.FirstOrDefault(n => n.Key == "RepetitiveBillingId");

			if (parameter.Key == null
				&& _editingRepetitiveBillingId == 0)
				return true;

			if (parameter.Key != null
				&& ((int)parameter.Value) == _editingRepetitiveBillingId)
				return true;

			return false;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			KeyValuePair<string, object> parameter;

			#region modification

			if ((parameter = navigationContext
					.Parameters
					.FirstOrDefault(n => n.Key == "RepetitiveBillingId")).Key != null)
			{
				var firstOpening = _editingRepetitiveBillingId == 0;

				_editingRepetitiveBillingId = (int)parameter.Value;

				var repetitiveBilling = _businessContext
					.RepetitiveBillingManager
					.GetRepetitiveBilling(_editingRepetitiveBillingId);

				#region mapping

				ValuationDate = repetitiveBilling.ValuationDate;
				Title = repetitiveBilling.Title;
				Amount = repetitiveBilling.Amount.ToString();
				Orientation = OrientationConverter.ConvertToEnum(repetitiveBilling.Positive);
				FrequenceMode = FrequenceModeConverter.ConvertToEnum(repetitiveBilling.FrequenceMode);

				#endregion

				if (firstOpening)
					_eventAggregator
						.GetEvent<DeletedRepetitiveBillingEvent>()
						.Subscribe(args => Close()
							, ThreadOption.UIThread
							, false
							, args => args.RepetitiveBillingId == _editingRepetitiveBillingId);
			}

			#endregion

			#region création

			else
			{
				ValuationDate = DateTime.Now;
				Title = "";
				Amount = "";
				Orientation = Orientation.Negative;
				FrequenceMode = FrequenceMode.Monthly;
			}

			#endregion
		}

		#endregion

		#region ITab

		public string HeaderTab
		{
			get { return "Formulaire échéancier"; }
		}

		public DelegateCommand CloseTabCommand
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
