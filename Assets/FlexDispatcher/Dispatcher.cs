using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlexFramework.EventSystem
{
    public static class Dispatcher
    {
        private static Handler _handler;
        private static readonly SubscriberCollection SubscriberCollection = new SubscriberCollection();
        private static readonly Queue<EventArgs> Events = new Queue<EventArgs>();
        private static readonly List<ScheduledEventArgs> ScheduledEvents = new List<ScheduledEventArgs>();
        private static event Action OneShotEvent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Init()
        {
            if (_handler)
                return;
            _handler = new GameObject("Dispatcher.Handler") { hideFlags = HideFlags.DontSave | HideFlags.NotEditable | HideFlags.HideInHierarchy }.AddComponent<Handler>();
            _handler.UpdateHandler += OnUpdate;
            _handler.UpdateHandler += OnOneShot;
        }

        private static void OnOneShot()
        {
            var handler = OneShotEvent;
            OneShotEvent = null;
            if (handler != null) handler();
        }

        private static void OnUpdate()
        {
            while (Events.Count > 0)
            {
                Dispatch(Events.Dequeue());
            }
            foreach (var @event in ScheduledEvents)
            {
                if (@event.Delay <= 0)
                    Dispatch(@event);
            }
            ScheduledEvents.RemoveAll(@event => @event.Delay <= 0);
            ScheduledEvents.ForEach(@event => @event.Tick(Time.deltaTime));
        }

        public static void Dispatch(EventArgs args)
        {
            using (SubscriberCollection)
            {
                foreach (var group in SubscriberCollection)
                {
                    group.Notify(args);
                }
            }
        }

        public static void Dispatch<T, U>(this T sender, U @event, params object[] data) where T : class where U : IConvertible
        {
            Dispatch(new EventArgs(sender, @event, data));
        }

        public static void Queue(QueuedEventArgs args)
        {
            Events.Enqueue(args);
        }

        public static void Queue<T, U>(this T sender, U @event, params object[] data) where T : class where U : IConvertible
        {
            Queue(new QueuedEventArgs(sender, @event, data));
        }

        public static void Schedule(ScheduledEventArgs args)
        {
            ScheduledEvents.Add(args);
        }

        public static void Schedule<T, U>(this T sender, U @event, float delay, params object[] data) where T : class where U : IConvertible
        {
            Schedule(new ScheduledEventArgs(sender, @event, delay, data));
        }

        public static void Resolve<T>(this T source) where T : class
        {
            if (SubscriberCollection.Locked)
            {
                OneShotEvent += () =>
                {
                    if (!SubscriberCollection.Contains(source))
                        SubscriberCollection.Add(source);
                };
            }
            else
            {
                if (!SubscriberCollection.Contains(source))
                    SubscriberCollection.Add(source);
            }
        }

        public static void Release<T>(this T source) where T : class
        {
            if (SubscriberCollection.Locked)
            {
                OneShotEvent += () =>
                {
                    SubscriberCollection.Remove(source);
                };
            }
            else
            {
                SubscriberCollection.Remove(source);
            }
        }

        public static void Release()
        {
            if (SubscriberCollection.Locked)
            {
                OneShotEvent += () =>
                {
                    SubscriberCollection.Clear();
                };
            }
            else
            {
                SubscriberCollection.Clear();
            }
        }

        private sealed class Handler : MonoBehaviour
        {
            public event Action UpdateHandler;

            private void Update()
            {
                if (UpdateHandler != null) UpdateHandler();
            }

            private void OnApplicationQuit()
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}