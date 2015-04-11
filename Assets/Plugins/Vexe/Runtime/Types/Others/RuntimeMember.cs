#if UNITY_EDITOR || UNITY_STANDALONE
#define DYNAMIC_REFLECTION
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;

namespace Vexe.Runtime.Types
{
    public class RuntimeMember
    {
#if DYNAMIC_REFLECTION
        private readonly MemberSetter<object, object> setter;
        private readonly MemberGetter<object, object> getter;
#else
        private readonly Action<object, object> setter;
        private readonly Func<object, object> getter;
#endif

        static Attribute[] Empty = new Attribute[0];

        public object Target;

        public Type Type { get; protected set; }

        public MemberInfo Info { get; private set; }

        public readonly bool IsStatic;

        public MemberTypes MemberType { get { return Info == null ? MemberTypes.Custom : Info.MemberType; } }

        public object Value
        {
            get { return Get(); }
            set { Set(value); }
        }

        protected Attribute[] attributes;
        public Attribute[] Attributes
        {
            get { return attributes ?? (attributes = Info == null ? Empty : Info.GetCustomAttributes<Attribute>().ToArray()); }
        }

        private string typeNiceName;
        public string TypeNiceName
        {
            get { return typeNiceName ?? (typeNiceName = Type.GetNiceName()); }
        }

        public string Name { get; protected set; }

        private string niceName;
        public string NiceName
        {
            get { return niceName ?? (niceName = Name.Replace("_", "").SplitPascalCase()); }
        }

        public RuntimeMember(MemberInfo member, object target)
        {
            if (member == null) return;

            Name     = member.Name;
            Info     = member;
            IsStatic = Info.IsStatic();
            Target   = target;

            var field = member as FieldInfo;
            if (field != null)
            {
                if (field.IsLiteral)
                    throw new InvalidOperationException("Can't wrap const fields " + field.Name);

                Type = field.FieldType;

#if DYNAMIC_REFLECTION
                setter = field.DelegateForSet();
                getter = field.DelegateForGet();
#else
                setter = field.SetValue;
                getter = field.GetValue;
#endif
            }
            else
            {
                var property = member as PropertyInfo;
                if (property == null)
                    throw new InvalidOperationException("MemberInfo {0} is not supported. Only fields and readable properties are".FormatWith(member));

                if (property.IsIndexer())
                    throw new InvalidOperationException("Can't wrap member {0} because it is an indexer.".FormatWith(member));

                if (!property.CanRead)
                    throw new InvalidOperationException("Property needs at least a public getter method to be wrapped as a Runtime/EditorMember " + member.Name);


                Type = property.PropertyType;

                if (property.CanWrite)
                {
#if DYNAMIC_REFLECTION
                    setter = property.DelegateForSet();
#else
                    setter = (x, y) => property.SetValue(x, y, null);
#endif
                }
#if DYNAMIC_REFLECTION
                else setter = delegate(ref object obj, object value) { };
#else
                else setter = (x, y) => { };
#endif

#if DYNAMIC_REFLECTION
                getter = property.DelegateForGet();
#else
                getter = x => property.GetValue(x, null);
#endif
            }
        }

        public virtual object Get()
        {
            return getter(Target);
        }

        public T As<T>() where T : class
        {
            return Get() as T;
        }

        public void Set(object target, object value)
        {
            Target = target;
            Set(value);
        }

        public virtual void Set(object value)
        {
#if DYNAMIC_REFLECTION
            setter(ref Target, value);
#else
            setter(Target, value);
#endif
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
            return member != null && member.Info == Info;
        }

        public static explicit operator FieldInfo(RuntimeMember member)
        {
            return member.Info as FieldInfo;
        }

        public static explicit operator PropertyInfo(RuntimeMember member)
        {
            return member.Info as PropertyInfo;
        }

        /// <summary>
        /// Returns a lazy enumerable DataMember representation of the specified member infos
        /// </summary>
        public static IEnumerable<RuntimeMember> Enumerate(IEnumerable<MemberInfo> members, object target)
        {
            foreach (var member in members)
            {
                var field = member as FieldInfo;
                if (field != null)
                {
                    if (field.IsLiteral)
                        continue;
                }
                else
                {
                    var prop = member as PropertyInfo;
                    if (prop == null || !prop.CanRead || prop.IsIndexer())
                        continue;
                }

                yield return new RuntimeMember(member, target);
            }
        }

        public static IEnumerable<RuntimeMember> Enumerate(Type type, object target, BindingFlags flags)
        {
            Assert.ArgumentNotNull(type, "type");
            return Enumerate(type.GetMembers(flags), target);
        }

        public static IEnumerable<RuntimeMember> Enumerate(Type type, object target)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return Enumerate(type, target, flags);
        }

        public static IEnumerable<RuntimeMember> Enumerate(Type type)
        {
            return Enumerate(type, null);
        }

        public static IEnumerable<RuntimeMember> Enumerate(object target, BindingFlags flags)
        {
            Assert.ArgumentNotNull(target, "target");
            return Enumerate(target.GetType(), target, flags);
        }

        private static Func<Type, List<RuntimeMember>> enumerateCached;
        public static Func<Type, List<RuntimeMember>> EnumerateCached
        {
            get {
                return enumerateCached ?? (enumerateCached = new Func<Type, List<RuntimeMember>>(type =>
                    RuntimeMember.Enumerate(type).ToList()).Memoize());
            }
        }
    }
}
