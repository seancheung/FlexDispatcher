using System;

namespace FlexFramework.EventSystem
{
    public abstract class Subscriber
    {
        protected readonly Type type;
        protected readonly object value;

        public Subscriber(Type type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public abstract void Notify(EventArgs args);
    }
}