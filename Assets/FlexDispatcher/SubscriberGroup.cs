namespace FlexFramework.EventSystem
{
    public class SubscriberGroup
    {
        protected readonly Subscriber[] subscribers;

        public int Count
        {
            get
            {
                return subscribers.Length;
            }
        }

        public SubscriberGroup(params Subscriber[] subscribers)
        {
            this.subscribers = subscribers;
        }

        public void Notify(EventArgs args)
        {
            foreach (var subscriber in subscribers)
            {
                subscriber.Notify(args);
            }
        }
    }
}