using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Runtime.Helpers
{
    public static class ReflectionHelper
    {
        public readonly static Func<Type, List<MemberInfo>> CachedGetMembers;

        readonly static Func<Tuple<Type, string>, MemberInfo> _getCachedMember;

        public static MemberInfo GetCachedMember(Type objType, string memberName)
        {
            return _getCachedMember(Tuple.Create(objType, memberName));
        }

        static ReflectionHelper()
        {
            CachedGetMembers = new Func<Type, List<MemberInfo>>(type =>
                GetMembers(type).ToList()).Memoize();

            _getCachedMember = new Func<Tuple<Type, string>, MemberInfo>(tup =>
            {
                var members = tup.Item1.GetMember(tup.Item2, Flags.StaticInstanceAnyVisibility);
                if (members.IsNullOrEmpty())
                    return null;
                return members[0];
            }).Memoize();
        }

        static IEnumerable<MemberInfo> GetMembers(Type type)
        {
            var peak = type.IsA<BetterBehaviour>() ? typeof(BetterBehaviour) : type.IsA<BetterScriptableObject>() ? typeof(BetterScriptableObject) : typeof(object);
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            var members = type.GetAllMembers(peak, flags);
            return members;
        }

        /// <summary>
        /// Returns the types of the specified members
        /// </summary>
        public static Type[] GetMembersTypes(MemberInfo[] members)
        {
            return members.Select(x => x.GetDataType()).ToArray();
        }

        /// <summary>
        /// Returns a reference to the unity engine assembly
        /// </summary>
        public static Assembly GetUnityEngineAssembly()
        {
            return typeof(UnityEngine.Object).Assembly;
        }

        /// <summary>
        /// Returns all runtime UnityEngine types
        /// </summary>
        public static Type[] GetAllUnityEngineTypes()
        {
            return GetUnityEngineAssembly().GetTypes();
        }

        /// <summary>
        /// Retruns all UnityEngine types of the specified wantedType
        /// </summary>
        public static Type[] GetAllUnityEngineTypesOf<T>()
        {
            return GetAllUnityEngineTypesOf(typeof(T));
        }

        /// <summary>
        /// Retruns all UnityEngine types of the specified wantedType
        /// </summary>
        public static Type[] GetAllUnityEngineTypesOf(Type wantedType)
        {
            return GetAllUnityEngineTypes().Where(t => wantedType.IsAssignableFrom(t)).ToArray();
        }

        /// <summary>
        /// Returns all user-types of the specified wantedType
        /// </summary>
        public static Type[] GetAllUserTypesOf<T>()
        {
            return GetAllUserTypesOf(typeof(T));
        }

        /// <summary>
        /// Returns all user-types of the specified wantedType
        /// </summary>
        public static Type[] GetAllUserTypesOf(Type wantedType)
        {
            return wantedType.Assembly.GetTypes().Where(t => wantedType.IsAssignableFrom(t)).ToArray();
        }

        /// <summary>
        /// Returns all types (user (and/or) UnityEngine types) of the specified wantedType
        /// </summary>
        public static Type[] GetAllTypesOf<T>()
        {
            return GetAllTypesOf(typeof(T));
        }

        /// <summary>
        /// Returns all types (user (and/or) UnityEngine types) of the specified wantedType
        /// </summary>
        public static Type[] GetAllTypesOf(Type wantedType)
        {
            return GetAllUserTypesOf(wantedType).Concat(GetAllUnityEngineTypesOf(wantedType)).ToArray();
        }
    }
}