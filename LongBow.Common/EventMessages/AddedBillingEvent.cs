using LongBow.Dom;

namespace LongBow.Common.EventMessages
{
	public class AddedBillingEvent : Microsoft.Practices.Prism.PubSubEvents.PubSubEvent<AddedBillingEventArgs>
	{
	}

	public class AddedBillingEventArgs
	{
		public Billing AddedBilling { get; set; }
	}
}
