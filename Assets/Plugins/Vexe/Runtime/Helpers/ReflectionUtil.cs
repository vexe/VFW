using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Runtime.Helpers
{
    public static class ReflectionUtil
    {
        public readonly static Func<Type, List<MemberInfo>> CachedGetMembers;

        readonly static Func<Tuple<Type, string>, MemberInfo> _getCachedMember;

        public static MemberInfo GetCachedMember(Type objType, string memberName)
        {
            return _getCachedMember(Tuple.Create(objType, memberName));
        }

        static ReflectionUtil()
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
    }
}
