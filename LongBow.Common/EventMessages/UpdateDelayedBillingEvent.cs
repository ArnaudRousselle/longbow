namespace LongBow.Common.EventMessages
{
	public class UpdateDelayedBillingEvent : Microsoft.Practices.Prism.PubSubEvents.PubSubEvent<UpdateDelayedBillingEventArgs>
	{
	}

	public class UpdateDelayedBillingEventArgs
	{
		public int BillingId { get; set; }
		public bool Delayed { get; set; }
	}
}
