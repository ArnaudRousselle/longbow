using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LongBow.Dom;

namespace LongBow.Common.EventMessages
{
	public class AddedRepetitiveBillingEvent : Microsoft.Practices.Prism.PubSubEvents.PubSubEvent<AddedRepetitiveBillingEventArgs>
	{
	}

	public class AddedRepetitiveBillingEventArgs
	{
		public RepetitiveBilling AddedRepetitiveBilling { get; set; }
	}
}
