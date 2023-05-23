using LongBow.Dom;
using LongBow.Dom.Constants;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace LongBow.CalendarListing
{
	public class DesignCalendarListingViewModel : ICalendarListingViewModel
	{
		private ObservableCollection<RepetitiveBillingVom> _repetitiveBillings;

		public DesignCalendarListingViewModel()
		{
			_repetitiveBillings = new ObservableCollection<RepetitiveBillingVom>();

			for (var i = 1; i < 21; i++)
			{
				_repetitiveBillings.Add(
					new RepetitiveBillingVom(
						new RepetitiveBilling
						{
							Id = i,
							ValuationDate = DateTime.Now.AddDays((i + 20)*(i%3 == 0 ? 1 : -1)),
							Title = "titre " + i,
							Amount = 50.2 + i + 1000*(i%2 == 0 ? 0 : 1),
							Positive = i%3 == 0,
							FrequenceMode = FrequenceModeConstant.Monthly,
						}));
			}
		}

		public double AverageMonthlyBalance
		{
			get { return 866.25; }
		}

		public ObservableCollection<RepetitiveBillingVom> RepetitiveBillings
		{
			get { return _repetitiveBillings; }
		}

		public DelegateCommand<object> CreateBillingCommand { get; set; }

        public DelegateCommand<IList> CreateMultipleBillingCommand { get; set; }

        public DelegateCommand CreateRepetitiveBillingCommand { get; set; }

		public DelegateCommand<object> EditRepetitiveBillingCommand { get; set; }

		public DelegateCommand<object> DeleteRepetitiveBillingCommand { get; set; }

		public InteractionRequest<IConfirmation> DeleteConfirmationRequest { get; set; }
	}
}
