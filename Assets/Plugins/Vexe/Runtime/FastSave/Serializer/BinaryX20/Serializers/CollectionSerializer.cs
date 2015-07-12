using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Vexe.Runtime.Extensions;

namespace BX20Serializer
{
    /// <summary>
    /// Intended mainly for generic: List, Stack, Queue
    /// </summary>
    public class CollectionSerializer : BaseSerializer
    {
        public override bool Handles(Type type)
        {
            return type.IsA<ICollection>() && GetAddMethod(type, GetElementType(type)) != null;
        }

        public override void Serialize(Stream stream, Type type, object value)
        {
            var collection = (ICollection)value;
            Type elementType = GetElementType(type);
            stream.WriteInt(collection.Count);

            foreach (var item in collection)
                ctx.Serializer.Serialize_Main(stream, elementType, item);
        }

        static object[] addArgs = new object[1];

        public override void Deserialize(Stream stream, Type type, ref object value)
        {
            var elementType = GetElementType(type);
            var add = GetAddMethod(type, elementType).DelegateForCall();
            var clear = GetClearMethod(type);
            clear.Invoke(value);

            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                object item = null;
                ctx.Serializer.Deserialize_Main(stream, elementType, ref item);
                addArgs[0] = item;
                add.Invoke(value, addArgs);
            }
        }

        public override object GetInstance(Stream stream, Type type)
        {
            return type.DelegateForCtor().Invoke(null);
        }

        static Type GetElementType(Type objectType)
        {
            if (objectType.HasElementType)
                return objectType.GetElementType();

            Type[] args = objectType.GetGenericArgsInRawParentInterface(typeof(ICollection<>));
            if (args != Type.EmptyTypes)
                return args[0];

            return typeof(object);
        }

        static Dictionary<Type, MethodInfo> _ClearMethods =
           new Dictionary<Type, MethodInfo>();

        static Dictionary<Type, MethodInfo> _AddMethods =
           new Dictionary<Type, MethodInfo>();

        static MethodInfo GetAddMethod(Type collection, Type element)
        {
            MethodInfo result;
            if (!_AddMethods.TryGetValue(collection, out result))
            {
                result =
                    GetAddMethod(collection, element, "Add") ??
                    GetAddMethod(collection, element, "Push") ??
                    GetAddMethod(collection, element, "Enqueue");
                _AddMethods[collection] = result;
            }

            if (result == null)
                throw new vMemberNotFound(collection, "Add/Push/Enqueue");

            return result;
        }

        static MethodInfo GetClearMethod(Type collection)
        {
            MethodInfo result;
            if (!_ClearMethods.TryGetValue(collection, out result))
                _ClearMethods[collection] = result = collection.GetMethod("Clear");

            if (result == null)
                throw new vMemberNotFound(collection, "Clear");

            return result;
        }

        static MethodInfo GetAddMethod(Type collection, Type element, string methodName)
        {
            var methods = collection.GetMember(methodName);
            for (int i = 0; i < methods.Length; i++)
            {
                var method = methods[i] as MethodInfo;
                if (method == null)
                    continue;
                var prams = method.GetParameters();
                if (prams.Length != 1)
                    continue;
                if (prams[0].ParameterType == element)
                    return method;
            }
            return null;
        }
    }
}
