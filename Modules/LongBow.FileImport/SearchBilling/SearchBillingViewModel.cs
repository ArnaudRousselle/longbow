using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Data;
using LongBow.Common.Interfaces.Bll;
using LongBow.Dom;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Mvvm;

namespace LongBow.FileImport.SearchBilling
{
    [Export(typeof(ISearchBillingViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchBillingViewModel : BindableBase, ISearchBillingViewModel, IInteractionRequestAware
    {
        #region Dependencies

        private readonly IBusinessContext _businessContext;

        #endregion

        #region Private fields

        private ISearchBillingConfirmation _confirmation;
        private readonly ICollectionView _billingsCollectionView;
        private DelegateCommand<Billing> _validateCommand;
        private string _researchedLabel;

        #endregion

        [ImportingConstructor]
        public SearchBillingViewModel(IBusinessContext businessContext)
        {
            _businessContext = businessContext;

            _billingsCollectionView = CollectionViewSource.GetDefaultView(Billings);

            _billingsCollectionView
                .SortDescriptions
                .Add(new SortDescription("ValuationDate", ListSortDirection.Ascending));
            
            _billingsCollectionView.CurrentChanged += (sender, args) =>
            {
                ValidateCommand.RaiseCanExecuteChanged();
            };

            _billingsCollectionView.Filter = o =>
            {
                var billing = o as Billing;

                if (billing == null)
                    return false;

                if (!string.IsNullOrEmpty(_researchedLabel)
                    && billing.Title.IndexOf(_researchedLabel, StringComparison.InvariantCultureIgnoreCase) == -1)
                    return false;

                return true;
            };
        }

        #region ISearchBillingViewModel

        public string ResearchedLabel
        {
            get { return _researchedLabel; }
            set
            {
                if (_researchedLabel == value)
                    return;

                _researchedLabel = value;
                OnPropertyChanged(() => ResearchedLabel);

                _billingsCollectionView.Refresh();
            }
        }

        public ObservableCollection<Billing> Billings { get; } = new ObservableCollection<Billing>();

        public DelegateCommand<Billing> ValidateCommand
        {
            get
            {
                return _validateCommand ??
                       (_validateCommand = new DelegateCommand<Billing>(
                           billing =>
                           {
                               _confirmation.Confirmed = true;
                               _confirmation.Billing = billing;
                               FinishInteraction();
                           },
                           billing => _billingsCollectionView.CurrentItem != null));
            }
        }

        #endregion

        #region IInteractionRequestAware

        public INotification Notification
        {
            get { return _confirmation; }
            set
            {
                _confirmation = value as ISearchBillingConfirmation;

                _researchedLabel = null;
                OnPropertyChanged(() => ResearchedLabel);

                Billings.Clear();

                Billings.AddRange(_businessContext
                    .BillingManager
                    .GetBillings()
                    .Where(n => !n.Checked));
            }
        }

        public Action FinishInteraction { get; set; }

        #endregion
    }
}
