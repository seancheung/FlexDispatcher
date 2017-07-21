using System;

namespace FlexFramework.EventSystem
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubscribeAttribute : Attribute
    {
        public readonly Type type;
        public readonly object value;

        public bool OneTime { get; set; }

        public SubscribeAttribute(Type eventType)
        {
            if (!typeof(IConvertible).IsAssignableFrom(eventType))
                throw new ArgumentException();
            type = eventType;
        }

        public SubscribeAttribute(object @event) : this(@event.GetType())
        {
            value = @event;
        }
    }
}