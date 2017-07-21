namespace FlexFramework.EventSystem
{
    public class QueuedEventArgs : EventArgs
    {
        public QueuedEventArgs(object sender, object @event, params object[] data) : base(sender, @event, data)
        {
        }
    }
}