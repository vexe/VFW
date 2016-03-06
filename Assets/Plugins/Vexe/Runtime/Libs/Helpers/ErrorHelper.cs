using System;
using System.Collections;
using UnityEngine;

namespace Vexe.Runtime.Helpers
{
    public static class ErrorHelper
    {
        public static void MemberNotFound(Type type, string name)
        {
            string message = string.Format("[MemberNotFound] Member {0} not found in type {1}", name, type);
            Log(message);
        }

        public static void TypeMismatch(Type expected, Type got)
        {
            string message = "[TypeMismatch] Expected type: `" + expected.Name + "` but got: `" + got.Name + "`";
            Log(message);
        }

        public static void InvalidCast(string fromTypeName, string toTypeName)
        {
            string message = "[InvalidCast] Cannot cast from `" + fromTypeName + "` to `" + toTypeName + "`";
            Log(message);
        }

        public static void InvalidCast(Type fromType, Type toType)
        {
            InvalidCast(fromType.Name, toType.Name);
        }

        public static void InvalidCast(object value, Type toType)
        {
            InvalidCast(value.GetType().Name, toType.Name);
        }

        public static void InvalidCast(object value, string toTypeName)
        {
            InvalidCast(value.GetType().Name, toTypeName);
        }

        public static void IndexOutOfRange(int outOfRangeIndex, int totalCount)
        {
            string message = "[IndexOutOfRange] Index `" + outOfRangeIndex + "` should be greater or equal to zero and less than the total count of `" + totalCount + "`";
            Log(message);
        }

        public static void KeyNotFound(string key)
        {
            string message = "[KeyNotFound] " + key;
            Log(message);
        }

        public static void IndexOutOfRange(int outOfRangeIndex, IList list)
        {
            IndexOutOfRange(outOfRangeIndex, list.Count);
        }

        public static void Log(string message)
        {
            Debug.LogError(message);
        }
    }
}
