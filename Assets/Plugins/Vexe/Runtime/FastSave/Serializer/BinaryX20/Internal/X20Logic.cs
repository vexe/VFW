using System;
using System.Linq;
using System.Reflection;

namespace BX20Serializer
{ 
    internal class X20Logic
    {
        public readonly Func<Type, MemberInfo[]> CachedGetSerializableMembers;

        private readonly FieldPredicate _IsSerializableField;
        private readonly PropertyPredicate _IsSerializableProperty;

        public X20Logic(FieldPredicate isSerializableField, PropertyPredicate isSerializableProperty)
        {
            _IsSerializableField = isSerializableField;
            _IsSerializableProperty = isSerializableProperty;

            CachedGetSerializableMembers = new Func<Type, MemberInfo[]>(type =>
            {
                var members = X20Reflection.CachedGetMembers(type);
                var serializableMembers = members.Where(IsSerializableMember).ToArray();
                return serializableMembers;
            }).Memoize();
        }

        public bool IsSerializableMember(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Method)
                return false;

            var field = member as FieldInfo;
            if (field != null)
                return _IsSerializableField(field);

            var property = member as PropertyInfo;
            if (property != null)
                return _IsSerializableProperty(property);

            return false;
        }

        public Type[] GetSerializableMembersTypes(Type forType)
        {
            var members = CachedGetSerializableMembers(forType);
            var result = X20Reflection.GetMembersTypes(members);
            return result;
        }
    }
}
