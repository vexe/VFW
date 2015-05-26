using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Helpers
{
    public static class ReflectionHelper
    {
        public static List<string> IgnoredAssemblies = new List<string>
        {
            "UnityScript.Lang",
            "Boo.Lang.Parser",
            "Boo.Lang",
            "Boo.Lang.Compiler",
            "System.ComponentModel.DataAnnotations",
            "System.Xml.Linq",
            "ICSharpCode.NRefactory",
            "UnityScript",
            "Mono.Cecil",
            "nunit.framework",
            "AssetStoreToolsExtra",
            "AssetStoreTools",
            "Unity.PackageManager",
            "Unity.SerializationLogic",
            "Mono.Security",
            "System.Xml",
            "System.Configuration",
            "System",
            "Unity.IvyParser",
            "System.Core",
            "Unity.DataContract",
            "I18N.West",
            "I18N",
            "Unity.Locator",
            "mscorlib",
            "nunit.core",
            "nunit.core.interfaces",
            "Mono.Cecil.Mdb",
            "NSubstitute",
            "UnityVS.VersionSpecific",
            "SyntaxTree.VisualStudio.Unity.Bridge",
            "SyntaxTree.VisualStudio.Unity.Messaging",
            "UnityEngine.UI",
            "UnityEngine",
            "FullSerializer",
        };

        public readonly static Func<Type, List<MemberInfo>> CachedGetMembers;
        public readonly static Func<Type[]> CachedGetRuntimeTypes;

        readonly static Func<Tuple<Type, string>, MemberInfo> _cachedGetMember;

        public static MemberInfo CachedGetMember(Type objType, string memberName)
        {
            return _cachedGetMember(Tuple.Create(objType, memberName));
        }

        static ReflectionHelper()
        {
            CachedGetMembers = new Func<Type, List<MemberInfo>>(type =>
                GetMembers(type).ToList()).Memoize();

            _cachedGetMember = new Func<Tuple<Type, string>, MemberInfo>(tup =>
            {
                var members = tup.Item1.GetMember(tup.Item2, Flags.StaticInstanceAnyVisibility);
                if (members.IsNullOrEmpty())
                    return null;
                return members[0];
            }).Memoize();

            CachedGetRuntimeTypes = new Func<Type[]>(() =>
            {
                Predicate<string> isIgnoredAssembly = name =>
                    name.Contains("Dbg") || name.Contains("Editor") || IgnoredAssemblies.Contains(name);

                return AppDomain.CurrentDomain.GetAssemblies()
                                              .Where(x => !isIgnoredAssembly(x.GetName().Name))
                                              .SelectMany(x => x.GetTypes())
                                              .ToArray();
            }).Memoize();
        }

        static IEnumerable<MemberInfo> GetMembers(Type type)
        {
            var peak = type.IsA<BetterBehaviour>() ? typeof(BetterBehaviour) : type.IsA<BetterScriptableObject>() ? typeof(BetterScriptableObject) : typeof(object);
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            var members = type.GetAllMembers(peak, flags);
            return members;
        }

        public static Type[] GetUnityEngineTypes()
        {
            return GetUnityEngineAssembly().GetTypes();
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
            return typeof(UnityObject).Assembly;
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
        public static Type[] GetAllUnityEngineTypesOf(Type type)
        {
            return GetAllUnityEngineTypes().Where(type.IsA).ToArray();
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
        public static Type[] GetAllUserTypesOf(Type type)
        {
            return CachedGetRuntimeTypes().Where(type.IsAssignableFrom).ToArray();
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

        /// <summary>
        /// Returns all types in all assemblies in the current domain
        /// </summary>
        public static IEnumerable<Type> GetAllTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
        }
    }
}