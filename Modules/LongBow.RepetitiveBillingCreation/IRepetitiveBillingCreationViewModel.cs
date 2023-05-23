using System;
using System.Collections.ObjectModel;
using LongBow.Common.Enumerations;
using Microsoft.Practices.Prism.Commands;

namespace LongBow.RepetitiveBillingCreation
{
	public interface IRepetitiveBillingCreationViewModel
	{
		DateTime ValuationDate { get; set; }
		string Title { get; set; }
		string Amount { get; set; }
		Orientation Orientation { get; set; }
		FrequenceMode FrequenceMode { get; set; }
		DelegateCommand ValidateCommand { get; }
	}
}
