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
using LongBow.Common.Interfaces.Views;
using LongBow.Common.NavigationParameter;
using LongBow.Dom;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Orientation = LongBow.Common.Enumerations.Orientation;

namespace LongBow.BillingCreation
{
	[Export(typeof (IEditingViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class EditingViewModel : BindableBase, IEditingViewModel, INavigationAware, ISwitchable, IClosable, INotifyDataErrorInfo, ITab, IViewRemovedAware
    {
		~EditingViewModel()
		{
			_loggerFacade.Log("EditingViewModel garbage collected " + EditingBillingId + " " + RepetitiveBillingId, Category.Debug, Priority.Low);
		}

        private readonly IBusinessContext _businessContext;
		private readonly IEventAggregator _eventAggregator;
		private readonly ILoggerFacade _loggerFacade;

		private readonly List<SubscriptionToken> _subscriptionTokens = new List<SubscriptionToken>();
		private bool _isDurty;
		private DateTime _transactionDate;
		private DateTime _valuationDate;
		private string _title;
		private string _amount;
		private Orientation _orientation;
		private bool _checked;
		private bool _isSaving;
		private bool _delayed;
		private string _comment;
		private bool _shiftValuationDate;
	    private bool _isSwitching;

		private DelegateCommand _validateCommand;
		private InteractionRequest<IConfirmation> _closeConfirmationRequest;
		private DelegateCommand _switchTabCommand;
		private DelegateCommand _closeTabCommand;

		[ImportingConstructor]
		public EditingViewModel(IBusinessContext businessContext,
			IEventAggregator eventAggregator,
			ILoggerFacade loggerFacade)
		{
			_businessContext = businessContext;
			_eventAggregator = eventAggregator;
			_loggerFacade = loggerFacade;

			EditingBillingId = null;
			RepetitiveBillingId = null;
		}

		public int? EditingBillingId { get; private set; }
		public int? RepetitiveBillingId { get; private set; }

		public DateTime TransactionDate
		{
			get { return _transactionDate; }
			set
			{
				if (!_isDurty) _isDurty = true;
				SetProperty(ref _transactionDate, value);
			}
		}

		public DateTime ValuationDate
		{
			get { return _valuationDate; }
			set
			{
				if (!_isDurty) _isDurty = true;
				SetProperty(ref _valuationDate, value);
			}
		}

		public string Title
		{
			get { return _title; }
			set
			{
				ClearErrors(() => Title);

				if (!_isDurty) _isDurty = true;
				
				if(string.IsNullOrWhiteSpace(value))
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

				if (!_isDurty) _isDurty = true;

				if (string.IsNullOrWhiteSpace(value))
					SetErrors(() => Amount, "Vous devez renseigner un montant");
				
				double d;
				if (!double.TryParse(value, out d))
					SetErrors(() => Amount, "Le montant doit être un nombre réel");

				if(d < 0)
					SetErrors(() => Amount, "Le montant doit être positif");

				SetProperty(ref _amount, value);
			}
		}

		public Orientation Orientation
		{
			get { return _orientation; }
			set
			{
				_orientation = value;
				if (!_isDurty) _isDurty = true;
				OnPropertyChanged(() => Orientation);
			}
		}

		public bool Checked
		{
			get { return _checked; }
			set
			{
				if (!_isDurty) _isDurty = true;
				SetProperty(ref _checked, value);
			}
		}

		public bool IsSaving
		{
			get { return _isSaving; }
			set
			{
				if (!_isDurty) _isDurty = true;
				SetProperty(ref _isSaving, value);
			}
		}

		public bool Delayed
		{
			get { return _delayed; }
			set
			{
				if (!_isDurty) _isDurty = true;
				SetProperty(ref _delayed, value);

				if (value)
				{
					var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

				    date = date.AddMonths(1).AddDays(-1);

                    if (date < DateTime.Now)
						date = date.AddMonths(1);

					ValuationDate = date;
				}
			}
		}

		public string Comment
		{
			get { return _comment; }
			set
			{
				if (!_isDurty) _isDurty = true;
				SetProperty(ref _comment, value);
			}
		}

		public bool ShiftValuationDate
		{
			get { return _shiftValuationDate; }
			set { SetProperty(ref _shiftValuationDate, value); }
		}

		public DelegateCommand ValidateCommand
		{
			get
			{
				return _validateCommand ?? (_validateCommand = new DelegateCommand(ValidateCommandMethod, () => !HasErrors));
			}
		}

		private void ValidateCommandMethod()
		{
			Billing billing;

			#region mapping

			billing = new Billing
			          {
				          TransactionDate = TransactionDate,
				          ValuationDate = ValuationDate,
				          Title = Title,
				          Amount = double.Parse(Amount),
						  Positive = OrientationConverter.ConvertToBool(Orientation),
				          Checked = Checked,
							IsSaving = IsSaving,
						  Delayed = Delayed,
				          Comment = Comment,
			          };

			#endregion

			if (EditingBillingId == null)
			{
				_businessContext.BillingManager.AddBilling(billing);

				if (RepetitiveBillingId != null && ShiftValuationDate)
				{
					_businessContext
						.RepetitiveBillingManager
						.ShiftRepetitiveBilling(RepetitiveBillingId.Value);

					ShiftValuationDate = false;
					Close();
				}
			}
			else
			{
				billing.Id = EditingBillingId.Value;

				_businessContext.BillingManager.UpdateBilling(billing);
			}

			_isDurty = false;
		}

		public InteractionRequest<IConfirmation> CloseConfirmationRequest
		{
			get { return _closeConfirmationRequest ?? (_closeConfirmationRequest = new InteractionRequest<IConfirmation>()); }
		}

		public DelegateCommand SwitchTabCommand
		{
			get
			{
			    return _switchTabCommand
			           ?? (_switchTabCommand = new DelegateCommand(
			               () =>
			               {
			                   _isSwitching = true;
                               Switch();
                               _isSwitching = false;
                           }));
			}
		}

		#region INavigationAware

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			var parameterBillingId = navigationContext.Parameters.FirstOrDefault(n => n.Key == "BillingId");
			var parameterRepetitiveBillingId = navigationContext.Parameters.FirstOrDefault(n => n.Key == "RepetitiveBillingId");

			if (parameterBillingId.Key == null
				&& EditingBillingId == null
				&& parameterRepetitiveBillingId.Key == null
				&& RepetitiveBillingId == null)
				return true;

			if (parameterBillingId.Key != null
				&& ((int) parameterBillingId.Value) == EditingBillingId)
				return true;

			if (parameterRepetitiveBillingId.Key != null
				&& ((int)parameterRepetitiveBillingId.Value) == RepetitiveBillingId)
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
					.FirstOrDefault(n => n.Key == "BillingId")).Key != null)
			{
				var firstOpening = EditingBillingId == null;

				EditingBillingId = (int)parameter.Value;

				var billing = _businessContext.BillingManager.GetBilling(EditingBillingId.Value);

				#region mapping

				_transactionDate = billing.TransactionDate;
				_valuationDate = billing.ValuationDate;
				_title = billing.Title;
				_amount = billing.Amount.ToString();
				_orientation = OrientationConverter.ConvertToEnum(billing.Positive);
				_checked = billing.Checked;
				_isSaving = billing.IsSaving;
				_delayed = billing.Delayed;
				_comment = billing.Comment;
				_shiftValuationDate = false;

				#endregion

				if (firstOpening)
					_subscriptionTokens.Add(
						_eventAggregator
							.GetEvent<DeletedBillingEvent>()
							.Subscribe(args => Close(),
								ThreadOption.UIThread
								, false
								, args => args.BillingId == EditingBillingId.Value));
			} 

			#endregion

			#region création à partir de l'échéancier

			else if ((parameter = navigationContext
					.Parameters
					.FirstOrDefault(n => n.Key == "RepetitiveBillingId")).Key != null)
			{
				RepetitiveBillingId = (int)parameter.Value;

				var repetitiveBilling = _businessContext
					.RepetitiveBillingManager
					.GetRepetitiveBilling(RepetitiveBillingId.Value);

				#region mapping

				_transactionDate = DateTime.Now;
				_valuationDate = repetitiveBilling.ValuationDate;
				_title = repetitiveBilling.Title;
				_amount = repetitiveBilling.Amount.ToString();
				_orientation = OrientationConverter.ConvertToEnum(repetitiveBilling.Positive);
				_checked = false;
				_isSaving = false;
				_delayed = false;
				_comment = "";
				_shiftValuationDate = true;

				#endregion

				_subscriptionTokens.Add(
					_eventAggregator
					.GetEvent<DeletedRepetitiveBillingEvent>()
					.Subscribe(args => Close()
						, ThreadOption.UIThread
						, false
						, args => args.RepetitiveBillingId == RepetitiveBillingId.Value));
			}

            #endregion

            #region création à partir des mails

            else if ((parameter = navigationContext
                    .Parameters
                    .FirstOrDefault(n => n.Key == "MailParameter")).Key != null)
            {
                var mailParameter = (MailParameter)parameter.Value;

                #region mapping

                _transactionDate = mailParameter.MailDate;
                _valuationDate = mailParameter.MailDate.AddDays(1);
                _title = mailParameter.Title;
                _amount = mailParameter.Amount.ToString();
                _orientation = Orientation.Negative;
                _checked = false;
				_isSaving = false;
				_delayed = false;
                _comment = "";
                _shiftValuationDate = true;

                #endregion

                _subscriptionTokens.Add(
                    _eventAggregator
                    .GetEvent<DeletedRepetitiveBillingEvent>()
                    .Subscribe(args => Close()
                        , ThreadOption.UIThread
                        , false
                        , args => args.RepetitiveBillingId == RepetitiveBillingId.Value));
            }

            #endregion

            #region création

            else
            {
				_transactionDate = DateTime.Now.Date;
				_valuationDate = DateTime.Now.Date;
				_title = "";
				_amount = "";
				_orientation = Orientation.Negative;
				_checked = false;
				_isSaving = false;
				_delayed = false;
				_comment = "";
				_shiftValuationDate = false;
			} 

			#endregion

			OnPropertyChanged(() => TransactionDate);
			OnPropertyChanged(() => ValuationDate);
			OnPropertyChanged(() => Title);
			OnPropertyChanged(() => Amount);
			OnPropertyChanged(() => Orientation);
			OnPropertyChanged(() => Checked);
			OnPropertyChanged(() => IsSaving);
			OnPropertyChanged(() => Delayed);
			OnPropertyChanged(() => Comment);
			OnPropertyChanged(() => ShiftValuationDate);

			_isDurty = false;

		    bool? autoInsert;

		    if ((autoInsert = navigationContext.Parameters["AutoInsert"] as bool?).HasValue
                && autoInsert.Value
                && !HasErrors)
		    {
		        ValidateCommandMethod();
		    }
		}

		#endregion

		#region IClosable

		public Action Close { get; set; }

		#endregion

		#region ISwitchable

		public Action Switch { get; set; }

		#endregion

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

		#region ITab

		public string HeaderTab
		{
			get
			{
				return (EditingBillingId != null
					? "Edition"
					: RepetitiveBillingId != null ? "Ajout (éch.)" : "Ajout");
			}
		}

		public DelegateCommand CloseTabCommand
		{
			get
			{
				return _closeTabCommand ?? (_closeTabCommand = new DelegateCommand(
					() =>
					{
						if (!_isDurty)
							Close();
						else
						{
							CloseConfirmationRequest.Raise(new Confirmation
							{
								Title = "Confirmer ?",
								Content = "Une modification est en cours. Confirmez-vous la fermeture ?"
							},
								confirmation =>
								{
									if (confirmation.Confirmed)
										Close();
								});
						}
					}));
			}
		}

        #endregion

        #region IViewRemovedAware

        public void OnViewRemovedFromRegion()
        {
            if (_isSwitching)
                return;

            foreach (var subscriptionToken in _subscriptionTokens.ToArray())
            {
                subscriptionToken.Dispose();
                _subscriptionTokens.Remove(subscriptionToken);
            }
        }

        #endregion
    }

}
