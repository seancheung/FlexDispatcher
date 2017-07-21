using System;
using UnityEngine;

namespace FlexFramework.EventSystem
{
    public static class Dispatcher
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Init()
        {
            throw new NotImplementedException();
        }

        public static void Dispatch(EventArgs args)
        {
            throw new NotImplementedException();
        }

        public static void Dispatch<T, U>(this T sender, U @event, params object[] data) where T : class where U : IConvertible
        {
            Dispatch(new EventArgs(sender, @event, data));
        }

        public static void Queue(QueuedEventArgs args)
        {
            throw new NotImplementedException();
        }

        public static void Queue<T, U>(this T sender, U @event, params object[] data) where T : class where U : IConvertible
        {
            Queue(new QueuedEventArgs(sender, @event, data));
        }

        public static void Schedule(ScheduledEventArgs args)
        {
            throw new NotImplementedException();
        }

        public static void Schedule<T, U>(this T sender, U @event, float delay, params object[] data) where T : class where U : IConvertible
        {
            Schedule(new ScheduledEventArgs(sender, @event, delay, data));
        }

        public static void Resolve<T>(this T source) where T : class
        {
            throw new NotImplementedException();
        }

        public static void Release<T>(this T source) where T : class
        {
            throw new NotImplementedException();
        }

        public static void Release()
        {
            throw new NotImplementedException();
        }
    }
}