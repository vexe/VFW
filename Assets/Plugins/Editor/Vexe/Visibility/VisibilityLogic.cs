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
    public static class VFWVisibilityLogic
    {
        public static readonly VisibilityAttributes Attributes;
        public static readonly Func<MemberInfo, bool> CachedIsVisibleMember;
        public static readonly Func<Type, List<MemberInfo>> CachedGetVisibleMembers;

        static VFWVisibilityLogic()
        {
            Attributes = VisibilityAttributes.Default;

            CachedIsVisibleMember = new Func<MemberInfo, bool>(IsVisibleMember).Memoize();

            CachedGetVisibleMembers = new Func<Type, List<MemberInfo>>(type =>
            {
                return ReflectionHelper.CachedGetMembers(type)
                                       .Where(IsVisibleMember)
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

        public static bool IsVisibleMember(MemberInfo member)
        {
            if (member is MethodInfo)
                return Attributes.Show.Any(member.IsDefined);

            var logic = VFWSerializationLogic.Instance;
            var field = member as FieldInfo;
            if (field != null)
                return !Attributes.Hide.Any(field.IsDefined)
                    && (logic.IsSerializableField(field) || Attributes.Show.Any(field.IsDefined));

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

            bool serializable = logic.IsSerializableProperty(property);
            if (serializable)
                return true;

            return Attributes.Show.Any(property.IsDefined);
        }

        static HashSet<string> IgnoredUnityProperties = new HashSet<string>()
        {
            "tag", "enabled", "name", "hideFlags"
        };
    }
}
