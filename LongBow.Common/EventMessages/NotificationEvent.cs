namespace LongBow.Common.EventMessages
{
    public class LongBowNotificationEvent : Microsoft.Practices.Prism.PubSubEvents.PubSubEvent<LongBowNotificationEventArgs>
    {
    }

    public class LongBowNotificationEventArgs
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
