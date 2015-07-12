using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BX20Serializer
{
    public static class X20Reflection
    {
        public static readonly Func<Type, MemberInfo[]> CachedGetMembers;

        static X20Reflection()
        {
            CachedGetMembers = new Func<Type, MemberInfo[]>(type =>
                GetAllMembers(type).ToArray()).Memoize();
        }

        public static MemberInfo GetMemberFromAll(this Type type, string memberName, Type peak, BindingFlags flags)
        {
            var result = GetAllMembers(type, peak, flags).FirstOrDefault(x => x.Name == memberName);
            return result;
        }

        public static MemberInfo GetMemberFromAll(this Type type, string memberName, BindingFlags flags)
        {
            var peak = type.IsA<MonoBehaviour>() ? typeof(MonoBehaviour)
                     : type.IsA<ScriptableObject>() ? typeof(ScriptableObject)
                     : typeof(object);

            return GetMemberFromAll(type, memberName, peak, flags);
        }

        public static IEnumerable<MemberInfo> GetAllMembers(this Type type, Type peak, BindingFlags flags)
        {
            if (type == null || type == peak)
                return Enumerable.Empty<MemberInfo>();

            return type.GetMembers(flags).Concat(GetAllMembers(type.BaseType, peak, flags));
        }

        public static IEnumerable<MemberInfo> GetAllMembers(this Type type, Type peak)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static;
            return GetAllMembers(type, peak, flags);
        }

        public static IEnumerable<MemberInfo> GetAllMembers(this Type type)
        {
            return GetAllMembers(type, typeof(object));
        }

        public static bool IsIndexer(this PropertyInfo property)
        {
            return property.GetIndexParameters().Length > 0;
        }

        public static Type[] GetGenericArgsInRawParentInterface(this Type type, Type rawParent)
        {
            if (!rawParent.IsGenericTypeDefinition)
                return Type.EmptyTypes;

            var interfaces = type.GetInterfaces();
            var parentInterface = interfaces.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == rawParent);
            if (parentInterface == null)
                return Type.EmptyTypes;

            return parentInterface.GetGenericArguments();
        }

        public static Type[] GetGenericArgsInRawParentClass(this Type type, Type rawParent)
        {
            if (!rawParent.IsGenericTypeDefinition)
                return Type.EmptyTypes;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == rawParent)
                return type.GetGenericArguments();

            Type baseType = type.BaseType;

            while (baseType != typeof(object) && baseType.GetGenericTypeDefinition() != rawParent)
                baseType = baseType.BaseType;

            return baseType == typeof(object) ? Type.EmptyTypes : baseType.GetGenericArguments();
        }

        public static bool HasConstructor<T>(this Type type)
        {
            return type.GetConstructor(new Type[] { typeof(T) }) != null;
        }

        public static ConstructorInfo GetEmptyConstructor(this Type type)
        {
            return GetEmptyConstructor(type, "No public empty ctor in type: " + type);
        }

        public static ConstructorInfo GetEmptyConstructor(this Type type, string msg)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
                throw new Exception(msg);
            return ctor;
        }

        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType)
        {
            while (toCheck != typeof(object) && toCheck != null)
            {
                Type current = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (current == baseType)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static bool IsImplementerOfRawGeneric(this Type type, Type baseType)
        {
            return type.GetInterfaces().Any(interfaceType =>
            {
                var current = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
                return current == baseType;
            });
        }

        public static Type[] GetMembersTypes(MemberInfo[] members)
        {
            return members.Select<MemberInfo, Type>(GetMemberType).ToArray();
        }

        public static MethodInfo GetMethodMarkedWith(this Type type, Type attribute)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var result = methods.FirstOrDefault(x => x.IsDefined(attribute));
            return result;
        }

        public static Type GetMemberType(this MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            var field = member as FieldInfo;
            if (field != null)
                return field.FieldType;

            var property = member as PropertyInfo;
            if (property != null)
                return property.PropertyType;

            throw new NotSupportedException("Unsupported member: " + member.Name);
        }

        public static bool IsA<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public static bool IsA(this Type type, Type other)
        {
            return other.IsAssignableFrom(type);
        }

        public static bool IsDefined(this MemberInfo member, Type type)
        {
            return member.IsDefined(type, false);
        }

        public static bool IsAutoProperty(this PropertyInfo property)
        {
            if (!(property.CanWrite && property.CanWrite))
                return false;

            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            string compilerGeneratedName = "<" + property.Name + ">";
            return property.DeclaringType.GetFields(flags).Any(f => f.Name.Contains(compilerGeneratedName));
        }

        public static Func<TIn, TOut> Memoize<TIn, TOut>(this Func<TIn, TOut> fn)
        {
            var dic = new Dictionary<TIn, TOut>();
            return _in =>
            {
                TOut _out;
                if (!dic.TryGetValue(_in, out _out))
                {
                    _out = fn(_in);
                    dic.Add(_in, _out);
                }
                return _out;
            };
        }

        public static MemberInfo[] GetMembersFromNames(Type type, string[] names)
        {
            var members = new MemberInfo[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                var all = type.GetMember(name);
                if (all.Length == 0)
                {
                    Debug.LogError("No member " + name + " found in type " + type.Name);
                    continue;
                }
                members[i] = all[0];
            }
            return members;
        }

    }
}
