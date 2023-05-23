using System;
using System.Collections.Generic;
using LongBow.Common.Interfaces.Bll;

namespace LongBow.Reporting
{
    public class DesignReportingViewModel : IReportingViewModel
    {
        public double? CurrentTotalBalance => 2533.2;

        public double? MonthBalance => 1559.35;

        public double? CheckedBalance => -1200.23;

        public List<DelayedSubTotal> TotalDelayedItems { get; }
            = new List<DelayedSubTotal>
            {
                new DelayedSubTotal {Year = 2017, Month = 3, Result = 540.56},
                new DelayedSubTotal {Year = 2017, Month = 4, Result = 220.5}
            };

        public List<DelayedSubTotal> CheckedDelayedItems { get; }
            = new List<DelayedSubTotal>
            {
                new DelayedSubTotal {Year = 2017, Month = 3, Result = 321.27},
                new DelayedSubTotal {Year = 2017, Month = 4, Result = 40.5}
            };

        public DateTime ReferenceDate
        {
            get { return DateTime.Now; }
            set { }
        }

    }
}
