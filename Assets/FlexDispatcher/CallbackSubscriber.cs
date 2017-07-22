#if UNITY_IOS || ENABLE_IL2CPP
#define AOT
#endif

using System;
using System.Collections;
using UnityEngine;

namespace FlexFramework.EventSystem
{
    public class CallbackSubscriber : Subscriber
    {
        protected readonly Action<EventArgs> callback;
        public readonly bool oneTime;
        public bool Invoked { get; private set; }

        protected CallbackSubscriber(SubscribeAttribute attribute) : base(attribute.type, attribute.value)
        {
            oneTime = attribute.OneTime;
        }

        public CallbackSubscriber(SubscribeAttribute attribute, Action callback) : this(attribute)
        {
            this.callback = args => callback();
        }

        public CallbackSubscriber(SubscribeAttribute attribute, Action<EventArgs> callback) : this(attribute)
        {
            this.callback = callback;
        }

#if AOT
        public CallbackSubscriber(SubscribeAttribute attribute, Delegate @delegate) : this(attribute)
        {
            this.callback = args => @delegate.DynamicInvoke(args.Data);
        }

        public CallbackSubscriber(SubscribeAttribute attribute, Delegate @delegate, MonoBehaviour target) : this(attribute)
        {
            this.callback = args =>
            {
                var @return = @delegate.DynamicInvoke(args.Data);
                if (@return is IEnumerator)
                    target.StartCoroutine((IEnumerator)@return);
            };
        }
#endif
        public CallbackSubscriber(SubscribeAttribute attribute, Action<object[]> callback) : this(attribute)
        {
            this.callback = args => callback(args.Data);
        }

        public CallbackSubscriber(SubscribeAttribute attribute, Func<IEnumerator> coroutine, MonoBehaviour target) : this(attribute)
        {
            this.callback = args => target.StartCoroutine(coroutine());
        }

        public CallbackSubscriber(SubscribeAttribute attribute, Func<EventArgs, IEnumerator> coroutine, MonoBehaviour target) : this(attribute)
        {
            this.callback = args => target.StartCoroutine(coroutine(args));
        }

        public CallbackSubscriber(SubscribeAttribute attribute, Func<object[], IEnumerator> coroutine, MonoBehaviour target) : this(attribute)
        {
            this.callback = args => target.StartCoroutine(coroutine(args.Data));
        }

        public override void Notify(EventArgs args)
        {
            if (args.Event.GetType() != type)
                return;
            if (Invoked && oneTime)
                return;
            if (value == null || value.Equals(args.Event))
            {
                callback(args);
                Invoked = true;
            }
        }
    }
}