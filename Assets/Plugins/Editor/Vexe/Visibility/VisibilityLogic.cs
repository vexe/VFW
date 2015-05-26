using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Serialization;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Visibility
{ 
    public static class VisibilityLogic
    {
        public static readonly VisibilityAttributes Attributes;

        static readonly Func<Tuple<Type, RuntimeMember[]>, List<MemberInfo>> _cachedGetVisibleMembers;
        static readonly Func<Type, List<MemberInfo>> _cachedGetDefaultVisibleMembers;

        public static List<MemberInfo> CachedGetVisibleMembers(Type type, RuntimeMember[] serialized)
        {
            if (serialized == null)
                return _cachedGetDefaultVisibleMembers(type);
            return _cachedGetVisibleMembers(Tuple.Create(type, serialized));
        }

        static VisibilityLogic()
        {
            Attributes = VisibilityAttributes.Default;

            _cachedGetDefaultVisibleMembers = new Func<Type, List<MemberInfo>>(type =>
            {
                return ReflectionHelper.CachedGetMembers(type)
                                       .Where(x => IsVisibleMember(x, null))
                                       .OrderBy<MemberInfo, float>(GetMemberDisplayOrder)
                                       .ToList();
            }).Memoize();

            _cachedGetVisibleMembers = new Func<Tuple<Type, RuntimeMember[]>, List<MemberInfo>>(tup =>
            {
                return ReflectionHelper.CachedGetMembers(tup.Item1)
                                       .Where(x => IsVisibleMember(x, tup.Item2))
                                       .OrderBy<MemberInfo, float>(GetMemberDisplayOrder)
                                       .ToList();
            }).Memoize();
        }

        public static float GetMemberDisplayOrder(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null && attribute.DisplayOrder.HasValue)
                return attribute.Order;

            switch (member.MemberType)
            {
                case MemberTypes.Field: return 100f;
                case MemberTypes.Property: return 200f;
                case MemberTypes.Method: return 300f;
                default: throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Determines whether or not 'member' should be visible in the inspector
        /// The logic goes like this:
        /// If member was a method it is visible only if it's annotated with [Show]
        /// If member was a field/property it is visible if
        /// 1- it's annotated with [Show]
        /// 2- OR if it's serializable
        /// To determine if a field/property is serializable:
        /// 1- If the 'serialized' member array is null, we use the default VFWSerializationLogic
        /// 2- otherwise we see if that field/property is contained within the serialized array
        /// </summary>
        public static bool IsVisibleMember(MemberInfo member, RuntimeMember[] serialized)
        {
            if (member is MethodInfo)
                return Attributes.Show.Any(member.IsDefined);

            bool useDefaultLogic = serialized == null;

            var field = member as FieldInfo;
            if (field != null)
            { 
                if (Attributes.Hide.Any(field.IsDefined))
                    return false;

                if (Attributes.Show.Any(field.IsDefined))
                    return true;

                if (useDefaultLogic)
                    return VFWSerializationLogic.Instance.IsSerializableField(field);

                return serialized.Contains(x => x.Name == field.Name);
            }

            var property = member as PropertyInfo;
            if (property == null || Attributes.Hide.Any(property.IsDefined))
                return false;

            // accept properties such as transform.position, rigidbody.mass, etc
            // exposing unity properties is useful when inlining objects via [Inline]
            // (which is the only use-case these couple of lines are meant for)
            var declType = property.DeclaringType;
            bool isValidUnityType = declType.IsA<Component>() && !declType.IsA<MonoBehaviour>();
            bool unityProp = isValidUnityType && property.CanReadWrite() && !IgnoredUnityProperties.Contains(property.Name);
            if (unityProp)
                return true;

            if (Attributes.Show.Any(property.IsDefined))
                return true;

            if (useDefaultLogic)
                return VFWSerializationLogic.Instance.IsSerializableProperty(property);

            return serialized.Contains(x => x.Name == property.Name);
        }

        static HashSet<string> IgnoredUnityProperties = new HashSet<string>()
        {
            "tag", "enabled", "name", "hideFlags"
        };
    }
}
