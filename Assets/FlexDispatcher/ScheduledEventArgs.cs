namespace FlexFramework.EventSystem
{
    /// <summary>
    /// Scheduled event arguments
    /// </summary>
    public class ScheduledEventArgs : QueuedEventArgs
    {
        /// <summary>
        /// Delay time
        /// </summary>
        /// <returns></returns>
        public float Delay { get; private set; }

        public ScheduledEventArgs(object sender, object @event, float delay, params object[] data) : base(sender, @event, data)
        {
            Delay = delay;
        }

        public void Tick(float delta)
        {
            Delay -= delta;
        }
    }
}