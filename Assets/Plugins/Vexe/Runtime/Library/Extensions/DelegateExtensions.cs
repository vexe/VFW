using System;
using System.Collections.Generic;
using System.Linq;

namespace Vexe.Runtime.Extensions
{
    public static class DelegateExtensions
    {
        /// <summary>
        /// Memoizes the specified func - returns the memoized version
        /// </summary>
        public static Func<TResult> Memoize<TResult>(this Func<TResult> getValue)
        {
            TResult value = default(TResult);
            bool hasValue = false;
            return () =>
            {
                if (!hasValue)
                {
                    hasValue = true;
                    value = getValue();
                }
                return value;
            };
        }

        /// <summary>
        /// Memoizes the specified func - returns the memoized version
        /// </summary>
        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func)
        {
            var dic = new Dictionary<T, TResult>();
            return n =>
            {
                TResult result;
                if (!dic.TryGetValue(n, out result))
                {
                    result = func(n);
                    dic.Add(n, result);
                }
                return result;
            };
        }

        public static bool HasHandler(this Action del, Action handler)
        {
            return del.GetInvocationList().Contains(handler);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static void SafeInvoke(this Action del)
        {
            if (del != null)
                del();
        }

        /// <summary>
        /// Invokes the delegate if it's not null with the specified argument
        /// </summary>
        public static void SafeInvoke<T>(this Action<T> del, T value)
        {
            if (del != null)
                del(value);
        }

        /// <summary>
        /// Invokes the delegate if it's not null with the specified arguments
        /// </summary>
        public static void SafeInvoke<T, U> (this Action<T, U> del, T value1, U value2)
        {
            if (del != null)
                del(value1, value2);
        }

        /// <summary>
        /// Invokes the delegate if it's not null with the specified arguments
        /// </summary>
        public static void SafeInvoke<T, U, V> (this Action<T, U, V> del, T value1, U value2, V value3)
        {
            if (del != null)
                del(value1, value2, value3);
        }

        /// <summary>
        /// Invokes the delegate if it's not null with the specified arguments
        /// </summary>
        public static void SafeInvoke<T, U, V, W> (this Action<T, U, V, W> del, T value1, U value2, V value3, W value4)
        {
            if (del != null)
                del(value1, value2, value3, value4);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static TReturn SafeInvoke<TReturn>(this Func<TReturn> del)
        {
            return del == null ? del() : default(TReturn);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static TReturn SafeInvoke<TReturn, T0>(this Func<T0, TReturn> del, T0 arg0)
        {
            return del == null ? del(arg0) : default(TReturn);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static TReturn SafeInvoke<TReturn, T0, T1>(this Func<T0, T1, TReturn> del, T0 arg0, T1 arg1)
        {
            return del == null ? del(arg0, arg1) : default(TReturn);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static TReturn SafeInvoke<TReturn, T0, T1, T2>(this Func<T0, T1, T2, TReturn> del, T0 arg0, T1 arg1, T2 arg2)
        {
            return del == null ? del(arg0, arg1, arg2) : default(TReturn);
        }

        /// <summary>
        /// Invokes the delegate if it's not null
        /// </summary>
        public static TReturn SafeInvoke<TReturn, T0, T1, T2, T3>(this Func<T0, T1, T2, T3, TReturn> del, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            return del == null ? del(arg0, arg1, arg2, arg3) : default(TReturn);
        }

        /// <summary>
        /// Returns true if this delegate doesn't have any handlers
        /// </summary>
        public static bool IsEmpty(this Delegate del)
        {
            if (del == null) return true;
            var list = del.GetInvocationList();
            return list.Length == 1 && list[0].Target == null;
        }
    }
}
