#if UNITY_EDITOR || UNITY_STANDALONE
#define DYNAMIC_REFLECTION
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;

namespace Vexe.Runtime.Types
{
    public class RuntimeMember
    {
#if DYNAMIC_REFLECTION
        private MemberSetter<object, object> _setter;
        private MemberGetter<object, object> _getter;
#else
        private Action<object, object> _setter;
        private Func<object, object> _getter;
#endif
        public object Target;
        public readonly string Name;
        public readonly string NiceName;
        public readonly string TypeNiceName;
        public readonly Type Type;
        public readonly MemberInfo Info;

        public object Value
        {
            get
            {
                return _getter(Target);
            }
            set
            {
#if DYNAMIC_REFLECTION
                _setter(ref Target, value);
#else
                _setter(Target, value);
#endif
            }
        }

        private RuntimeMember(MemberInfo memberInfo, Type memberType, object memberTarget)
        {
            Info = memberInfo;
            Type = memberType;
            Target = memberTarget;
            Name = memberInfo.Name;
            NiceName = Name.Replace("_", "").SplitPascalCase();
            TypeNiceName = memberType.GetNiceName();
        }

        public static bool TryWrapField(FieldInfo field, object target, out RuntimeMember result)
        {
            if (field.IsLiteral)
            { 
                result = null;
                return false;
            }

            result = new RuntimeMember(field, field.FieldType, target);

#if DYNAMIC_REFLECTION
            result._setter = field.DelegateForSet();
            result._getter = field.DelegateForGet();
#else
            result._setter = field.SetValue;
            result._getter = field.GetValue;
#endif
            return true;
        }

        public static bool TryWrapProperty(PropertyInfo property, object target, out RuntimeMember result)
        {
            if (!property.CanRead || property.IsIndexer())
            {
                result = null;
                return false;
            }

            result = new RuntimeMember(property, property.PropertyType, target);

            if (property.CanWrite)
            {
#if DYNAMIC_REFLECTION
                result._setter = property.DelegateForSet();
#else
                result._setter = (x, y) => property.SetValue(x, y, null);
#endif
            }
#if DYNAMIC_REFLECTION
            else result._setter = delegate(ref object obj, object value) { };
#else
            else result._setter = (x, y) => { };
#endif

#if DYNAMIC_REFLECTION
            result._getter = property.DelegateForGet();
#else
            result._getter = x => property.GetValue(x, null);
#endif
            return true;
        }

        public static List<RuntimeMember> WrapMembers(IEnumerable<MemberInfo> members, object target)
        {
            var result = new List<RuntimeMember>();
            foreach (var member in members)
            {
                var field = member as FieldInfo;
                if (field != null)
                {
                    RuntimeMember wrappedField;
                    if (RuntimeMember.TryWrapField(field, target, out wrappedField))
                        result.Add(wrappedField);
                }
                else
                {
                    var property = member as PropertyInfo;
                    if (property == null)
                        continue;

                    RuntimeMember wrappedProperty;
                    if (RuntimeMember.TryWrapProperty(property, target, out wrappedProperty))
                        result.Add(wrappedProperty);
                }
            }
            return result;
        }

        private static Func<Type, List<RuntimeMember>> _cachedWrapMembers;
        public static List<RuntimeMember> CachedWrapMembers(Type type)
        {
            if (_cachedWrapMembers == null)
                _cachedWrapMembers = new Func<Type, List<RuntimeMember>>(x =>
                {
                    var members = ReflectionHelper.CachedGetMembers(x);
                    return RuntimeMember.WrapMembers(members, null);
                }).Memoize();
            return _cachedWrapMembers(type);
        }

        public override string ToString()
        {
            return TypeNiceName + " " + Name;
        }

        public override int GetHashCode()
        {
            return Info.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var member = obj as RuntimeMember;
            return member != null && this.Info == member.Info;
        }
    }
}
