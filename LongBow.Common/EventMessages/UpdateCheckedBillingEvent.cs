namespace LongBow.Common.EventMessages
{
	public class UpdateCheckedBillingEvent : Microsoft.Practices.Prism.PubSubEvents.PubSubEvent<UpdateCheckedBillingEventArgs>
	{
	}

	public class UpdateCheckedBillingEventArgs
	{
		public int BillingId { get; set; }
		public bool Checked { get; set; }
	}
}
