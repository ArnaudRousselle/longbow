namespace LongBow.Common.EventMessages
{
	public class DeletedRepetitiveBillingEvent : Microsoft.Practices.Prism.PubSubEvents.PubSubEvent<DeletedRepetitiveBillingEventArgs>
	{
	}

	public class DeletedRepetitiveBillingEventArgs
	{
		public int RepetitiveBillingId { get; set; }
	}
}
