namespace FlexFramework.EventSystem
{
    public class ScheduledEventArgs : QueuedEventArgs
    {
        public ScheduledEventArgs(object sender, object @event, float delay, params object[] data) : base(sender, @event, data)
        {
        }
    }
}