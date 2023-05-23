using System;
using System.Collections.Generic;
using System.Globalization;
using LongBow.Dom;

namespace LongBow.Common.Interfaces.Bll
{
	public interface IBillingCalculationManager
	{
		BillingCalculationResult Calculate(List<Billing> billings);
		RepetitiveCalculationResult Calculate(List<RepetitiveBilling> repetitiveBillings);
	}

	public class BillingCalculationResult
	{
		public double CurrentTotalBalance { get; set; }
		public double CheckedBalance { get; set; }
	    public List<DelayedSubTotal> TotalDelayedItems { get; set; }
	    public List<DelayedSubTotal> CheckedDelayedItems { get; set; }
	}

    public class DelayedSubTotal
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double Result { get; set; }
        public string Name => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    }

    public class RepetitiveCalculationResult
	{
		public double AverageMonthlyBalance { get; set; }
	}
}
