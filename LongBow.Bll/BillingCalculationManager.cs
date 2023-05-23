using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using LongBow.Common.Interfaces.Bll;
using LongBow.Dom;
using LongBow.Dom.Constants;

namespace LongBow.Bll
{
	[Export(typeof(IBillingCalculationManager))]
	public class BillingCalculationManager : IBillingCalculationManager
	{
		public BillingCalculationResult Calculate(List<Billing> billings)
		{
			var currentTotalBalance = billings
				.Sum(n => n.Amount * (n.Positive ? 1 : -1));

			var checkedBalance = billings
				.Where(n => n.Checked && !n.Delayed)
				.Sum(n => n.Amount * (n.Positive ? 1 : -1));

		    var totalDelayedItems = new List<DelayedSubTotal>();

		    foreach (var grp in billings
		        .Where(n => n.Delayed)
		        .GroupBy(n => new {n.ValuationDate.Year, n.ValuationDate.Month})
                .OrderBy(n => n.Key.Year)
                .ThenBy(n => n.Key.Month))
		    {
		        totalDelayedItems.Add(new DelayedSubTotal
		        {
		            Year = grp.Key.Year,
		            Month = grp.Key.Month,
		            Result = grp.Sum(n => n.Amount*(n.Positive ? 1 : -1))
		        });
		    }

		    var checkedDelayedItems = new List<DelayedSubTotal>();

		    foreach (var grp in billings
		        .Where(n => n.Checked && n.Delayed)
		        .GroupBy(n => new {n.ValuationDate.Year, n.ValuationDate.Month})
		        .OrderBy(n => n.Key.Year)
		        .ThenBy(n => n.Key.Month))
		    {
				checkedDelayedItems.Add(new DelayedSubTotal
				{
					Year = grp.Key.Year,
					Month = grp.Key.Month,
					Result = grp.Sum(n => n.Amount * (n.Positive ? 1 : -1))
				});
			}

			return new BillingCalculationResult
			       {
					   CurrentTotalBalance = currentTotalBalance,
					   CheckedBalance = checkedBalance,
					   TotalDelayedItems = totalDelayedItems,
					   CheckedDelayedItems = checkedDelayedItems,
			       };
		}

		public RepetitiveCalculationResult Calculate(List<RepetitiveBilling> repetitiveBillings)
		{
			var res = new RepetitiveCalculationResult
			          {
				          AverageMonthlyBalance = 0
			          };

			if (repetitiveBillings.Count == 0)
				return res;

			foreach (var rb in repetitiveBillings)
			{
				int divisor;

				switch (rb.FrequenceMode)
				{
					case FrequenceModeConstant.Annual:
						divisor = 12;
						break;
					case FrequenceModeConstant.Quarterly:
						divisor = 3;
						break;
					case FrequenceModeConstant.Bimonthly:
						divisor = 2;
						break;
					case FrequenceModeConstant.Monthly:
					default:
						divisor = 1;
						break;
				}

				res.AverageMonthlyBalance += (rb.Positive ? 1 : -1) * rb.Amount / divisor;
			}

			return res;
		}
	}
}
