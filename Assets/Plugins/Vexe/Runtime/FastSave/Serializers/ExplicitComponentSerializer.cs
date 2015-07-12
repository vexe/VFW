using System;
using System.Collections.Generic;
using System.Reflection;
using BX20Serializer;

namespace Vexe.FastSave.Serializers
{
    public class ExplicitComponentSerializer : ReflectiveComponentSerializer
    {
        public readonly Dictionary<Type, string[]> SupportedComponentTypes = new Dictionary<Type, string[]>();

        readonly Dictionary<Type, MemberInfo[]> _CachedMembers = new Dictionary<Type, MemberInfo[]>();

        public ExplicitComponentSerializer Add(Type type, params string[] members)
        {
            SupportedComponentTypes.Add(type, members);
            return this;
        }

        public ExplicitComponentSerializer Add<T>(params string[] members)
        {
            return Add(typeof(T), members);
        }

        public override MemberInfo[] GetMembers(Type type)
        {
            MemberInfo[] members;
            if (!_CachedMembers.TryGetValue(type, out members))
            {
                var names = SupportedComponentTypes[type];
                members = X20Reflection.GetMembersFromNames(type, names);
                _CachedMembers[type] = members;
            }
            return members;
        }

        public override bool Handles(Type type)
        {
            return SupportedComponentTypes.ContainsKey(type);
        }
    }
}
