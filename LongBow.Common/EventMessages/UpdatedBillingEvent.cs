using LongBow.Dom;

namespace LongBow.Common.EventMessages
{
	public class UpdatedBillingEvent : Microsoft.Practices.Prism.PubSubEvents.PubSubEvent<UpdatedBillingEventArgs>
	{
	}

	public class UpdatedBillingEventArgs
	{
		public Billing UpdatedBilling { get; set; }
	}
}
