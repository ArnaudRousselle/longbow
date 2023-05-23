using LongBow.Dom;

namespace LongBow.Common.EventMessages
{
	public class UpdatedRepetitiveBillingEvent : Microsoft.Practices.Prism.PubSubEvents.PubSubEvent<UpdatedRepetitiveBillingEventArgs>
	{
	}

	public class UpdatedRepetitiveBillingEventArgs
	{
		public RepetitiveBilling UpdatedRepetitiveBilling { get; set; }
	}
}
