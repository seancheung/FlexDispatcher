#if UNITY_IOS || ENABLE_IL2CPP
#define AOT
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FlexFramework.EventSystem
{
    public class SubscriberCollection : IEnumerable<SubscriberGroup>, IDisposable
    {
        protected static readonly Dictionary<int, SubscriberGroup> Dictionary =
            new Dictionary<int, SubscriberGroup>();

        public bool Locked { get; private set; }

        public int Count
        {
            get { return Dictionary.Count; }
        }

        public void Add<T>(T source) where T : class
        {
            if (Locked)
                throw new InvalidOperationException();
            Dictionary.Add(source.GetHashCode(), Wrap(source));
        }

        public void Remove(int hash)
        {
            if (Locked)
                throw new InvalidOperationException();
            Dictionary.Remove(hash);
        }

        public void Remove<T>(T source) where T : class
        {
            Remove(source.GetHashCode());
        }

        public void Clear()
        {
            if (Locked)
                throw new InvalidOperationException();
            Dictionary.Clear();
        }

        public bool Contains<T>(T source) where T : class
        {
            return Contains(source.GetHashCode());
        }

        public bool Contains(int hash)
        {
            return Dictionary.ContainsKey(hash);
        }

        protected SubscriberGroup Wrap<T>(T source) where T : class
        {
            return new SubscriberGroup(Extract(source).ToArray());
        }

        protected IEnumerable<Subscriber> Extract<T>(T source) where T : class
        {
            var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var method in methods)
            {
                if (!method.IsDefined(typeof(SubscribeAttribute), true))
                    continue;
                if (method.IsGenericMethod || method.IsAbstract)
                    throw new NotSupportedException(string.Format("Generic/abstract method is not supported: {0}.{1}", typeof(T).FullName, method.Name));
                if (method.ReturnType != typeof(void))
                {
                    if (method.ReturnType != typeof(IEnumerator))
                        throw new NotSupportedException(string.Format("Only method with return type of void or IEnumerator is supported: {0}.{1}", typeof(T).FullName, method.Name));
                    if (!(source is MonoBehaviour))
                        throw new NotSupportedException(string.Format("Coroutine is only supported on MonoBehaviour: {0}.{1}", typeof(T).FullName, method.Name));
                }

                var attributes = Attribute.GetCustomAttributes(method, typeof(SubscribeAttribute), true).Cast<SubscribeAttribute>().ToArray();
                var parameters = method.GetParameters();

                foreach (var attribute in attributes)
                {
                    if (method.ReturnType == typeof(void))
                    {
                        if (parameters.Length == 0)
                            yield return new CallbackSubscriber(attribute, method.MakeDeletgate<T, Action>(source));
                        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(EventArgs))
                            yield return new CallbackSubscriber(attribute, method.MakeDeletgate<T, Action<EventArgs>>(source));
                        else
                            yield return new CallbackSubscriber(attribute, method.MakeAction(source));
                    }
                    else
                    {
                        if (parameters.Length == 0)
                            yield return new CallbackSubscriber(attribute, method.MakeDeletgate<T, Func<IEnumerator>>(source), source as MonoBehaviour);
                        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(EventArgs))
                            yield return new CallbackSubscriber(attribute, method.MakeDeletgate<T, Func<EventArgs, IEnumerator>>(source), source as MonoBehaviour);
                        else
                            yield return new CallbackSubscriber(attribute, method.MakeFunc(source), source as MonoBehaviour);
                    }
                }
            }
        }

        void IDisposable.Dispose()
        {
            Locked = false;
        }

        IEnumerator<SubscriberGroup> IEnumerable<SubscriberGroup>.GetEnumerator()
        {
            if (Locked)
                throw new InvalidOperationException();
            Locked = true;
            return Dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<SubscriberGroup>)this).GetEnumerator();
        }
    }
}