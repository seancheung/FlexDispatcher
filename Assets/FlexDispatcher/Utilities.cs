#if UNITY_IOS || ENABLE_IL2CPP
#define AOT
#endif

using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FlexFramework.EventSystem
{
    public static class Utilities
    {
        public static TDelegate MakeDeletgate<T, TDelegate>(this MethodInfo method, T target) where T : class where TDelegate : class
        {
            return Delegate.CreateDelegate(typeof(TDelegate), method.IsStatic ? null : target, method) as TDelegate;
        }

#if AOT
        public static Delegate MakeAction<T>(this MethodInfo method, T target) where T : class
        {
            var type = Expression.GetActionType(method.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
            return Delegate.CreateDelegate(type, method.IsStatic ? null : target, method);
        }

        public static Delegate MakeFunc<T>(this MethodInfo method, T target) where T : class
        {
            var type = Expression.GetFuncType(method.GetParameters().Select(parameter => parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray());
            return Delegate.CreateDelegate(type, method.IsStatic ? null : target, method);
        }
#else
        public static Action<object[]> MakeAction<T>(this MethodInfo method, T target) where T : class
        {
            var obj = Expression.Parameter(typeof(object), "target");
            var parameters = method.GetParameters();
            var array = Expression.Parameter(typeof(object[]), "args");
            var converted = parameters.Select(p => Expression.Convert(Expression.ArrayIndex(array, Expression.Constant(p.Position)), p.ParameterType)).ToArray();
            var body = Expression.Call(Expression.Convert(obj, method.DeclaringType), method, converted);
            var @delegate = Expression.Lambda<Action<object, object[]>>(body, obj, array).Compile();
            return args => @delegate(target, args);
        }

        public static Func<object[], IEnumerator> MakeFunc<T>(this MethodInfo method, T target) where T : class
        {
            var obj = Expression.Parameter(typeof(object), "target");
            var parameters = method.GetParameters();
            var array = Expression.Parameter(typeof(object[]), "args");
            var converted = parameters.Select(p => Expression.Convert(Expression.ArrayIndex(array, Expression.Constant(p.Position)), p.ParameterType)).ToArray();
            var body = Expression.Call(Expression.Convert(obj, method.DeclaringType), method, converted);
            var @delegate = Expression.Lambda<Func<object, object[], IEnumerator>>(body, obj, array).Compile();
            return args => @delegate(target, args);
        }
#endif

    }
}