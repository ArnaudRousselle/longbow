namespace LongBow.Common.EventMessages
{
	public class DeletedBillingEvent : Microsoft.Practices.Prism.PubSubEvents.PubSubEvent<DeletedBillingEventArgs>
	{
	}

	public class DeletedBillingEventArgs
	{
		public int BillingId { get; set; }
	}
}
