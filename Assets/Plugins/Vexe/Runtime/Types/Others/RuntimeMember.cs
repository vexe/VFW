#if UNITY_EDITOR || UNITY_STANDALONE
//#define DYNAMIC_REFLECTION
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;

namespace Vexe.Runtime.Types
{
    /// <summary>
    /// A wrapper for metadata members (fields and properties)
    /// that's used to conveneitly set/get the member value on a certain target.
    /// The way setting/getting of members is done If you're in-editor or targetting standalone
    /// is via dynamically generated delegates (which is pretty fast)
    /// otherwise via standard reflection (slower)
    /// </summary>
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

        /// <summary>
        /// The name of the wrapped member
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// If the member name was "_someValue" or "someValue" then the nice name would be "Some Value"
        /// </summary>
        public readonly string NiceName;

        /// <summary>
        /// Say the member was a dictionary of float and string, the type nice name would be Dictionary<float, string>
        /// instead of System.Collections.Generic.Dictionary<Sys.. ahhh to hell with this!>
        /// </summary>
        public readonly string TypeNiceName;

        /// <summary>
        /// The type of the wrapped member (FieldInfo.FieldType in case of a field, or PropertyInfo.PropertyType in case of a property)
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// A reference to the MemberInfo reference of the wrapped member
        /// </summary>
        public readonly MemberInfo Info;

        /// <summary>
        /// The current value of the member in the current target object
        /// </summary>
        public object Value
        {
            get
            {
                return _getter(Target);
            }
            set
            {
                try
                {
#if DYNAMIC_REFLECTION
                    _setter(ref Target, value);
#else
                    _setter(Target, value);
#endif
                }
                catch(InvalidCastException)
                {
                    throw new vInvalidCast(value, TypeNiceName);
                }
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

        /// <summary>
        /// Returns false if the field was constant (literal) while setting 'result' to null.
        /// Otherwise true while setting result to a new RuntimeMember wrapping the specified field
        /// using the appropriate method of building the [s|g]etters (delegates in case of editor/standalone, reflection otherwise)
        /// </summary>
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

        /// <summary>
        /// Returns false if the property isn't readable or if it's an indexer, setting 'result' to null in the process.
        /// Otherwise true while setting result to a new RuntimeMember wrapping the specified property
        /// using the appropriate method of building the [s|g]etters (delegates in case of editor/standalone, reflection otherwise)
        /// Note that readonly properties (getter only) are fine, as the setter will just be an empty delegate doing nothing.
        /// </summary>
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

        /// <summary>
        /// Returns a list of RuntimeMember wrapping whatever is valid from the input members IEnumerable in the specified target
        /// </summary>
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
        /// <summary>
        /// A cached overload of WrapMembers that returns a list of RuntimeMembers with no target specified (null)
        /// wrapping whatever valid members there are in the specified type argument
        /// (uses cached reflection to get the members)
        /// </summary>
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
