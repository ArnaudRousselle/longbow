using System;
using System.Collections.Generic;
using LongBow.Common.Interfaces.Bll;

namespace LongBow.Reporting
{
	public interface IReportingViewModel
	{
		double? CurrentTotalBalance { get; } // solde total courant
		double? CheckedBalance { get; } // solde pointé
        List<DelayedSubTotal> TotalDelayedItems { get; } // en cours (par mois)
        List<DelayedSubTotal> CheckedDelayedItems { get; } // en cours pointé (par mois)
	}
}
