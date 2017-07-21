using System;

namespace FlexFramework.EventSystem
{
    public class EventArgs
    {
        public object Sender { get; private set; }

        public object Event { get; private set; }

        public object[] Data { get; private set; }

        public EventArgs(object sender, object @event, params object[] data)
        {
            if (sender == null)
                throw new ArgumentNullException();
            if (@event == null)
                throw new ArgumentNullException();
            Sender = sender;
            Event = @event;
            Data = data;
        }

        public T GetUnderlyingEvent<T>()
        {
            if (Event is T)
                return (T)Event;
            throw new InvalidCastException();
        }
    }
}