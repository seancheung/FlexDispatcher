using System;

namespace FlexFramework.EventSystem
{
    /// <summary>
    /// Event arguments
    /// </summary>
    public class EventArgs
    {
        /// <summary>
        /// Event sender
        /// </summary>
        public object Sender { get; private set; }

        /// <summary>
        /// Event
        /// </summary>
        /// <remarks>
        /// The event must be <see cref="IConvertible"/>
        /// </remarks>
        public object Event { get; private set; }

        /// <summary>
        /// Arguments passed to subscribers
        /// </summary>
        /// <returns></returns>
        public object[] Data { get; private set; }

        public EventArgs(object sender, object @event, params object[] data)
        {
            if (sender == null)
                throw new ArgumentNullException();
            if (@event == null)
                throw new ArgumentNullException();
            if (!(@event is IConvertible))
                throw new ArgumentException();
            Sender = sender;
            Event = @event;
            Data = data;
        }

        /// <summary>
        /// Cast event to target type if possibble
        /// </summary>
        /// <returns>Event</returns>
        public T Cast<T>() where T : IConvertible
        {
            if (Event is T)
                return (T)Event;
            throw new InvalidCastException();
        }
    }
}