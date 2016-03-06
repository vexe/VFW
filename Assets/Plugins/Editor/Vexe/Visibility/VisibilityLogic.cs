using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Visibility
{
    public static class VisibilityLogic
    {
        public static readonly VisibilityAttributes Attributes;

        static readonly Func<Type, List<MemberInfo>> _cachedGetDefaultVisibleMembers;

        public static List<MemberInfo> CachedGetVisibleMembers(Type type)
        {
            return _cachedGetDefaultVisibleMembers(type);
        }

        static VisibilityLogic()
        {
            Attributes = VisibilityAttributes.Default;

            _cachedGetDefaultVisibleMembers = new Func<Type, List<MemberInfo>>(type =>
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

        /// <summary>
        /// Determines whether or not 'member' should be visible in the inspector
        /// The logic goes like this:
        /// If member was a method or property it is visible only if it's annotated with [Show]
        /// If member was a field it is visible if
        /// 1- it's annotated with [Show]
        /// 2- OR if it's serializable
        /// </summary>
        public static bool IsVisibleMember(MemberInfo member)
        {
            if (member is MethodInfo)
                return Attributes.Show.Any(member.IsDefined);

            var field = member as FieldInfo;
            if (field != null)
            {
                if (Attributes.Hide.Any(field.IsDefined))
                    return false;

                if (Attributes.Show.Any(field.IsDefined))
                    return true;

                if (field.IsDefined<SerializeField>())
                    return true;

                return IsSerializableField(field);
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

            return false;
        }

        static HashSet<string> IgnoredUnityProperties = new HashSet<string>()
        {
            "tag", "enabled", "name", "hideFlags"
        };

        public static bool IsSerializableType(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || type == typeof(string)
                || type.IsA<UnityObject>()
                || IsUnityStructType(type))
                return true;

            if (type.IsArray)
                return type.GetArrayRank() == 1 && IsSerializableType(type.GetElementType());

            if (type.IsInterface)
                return true;

            if (type.IsA<Delegate>())
                return false;

            if (type.IsA<Type>())
                return true;

            if (type.IsGenericType)
                return type.GetGenericArguments().All(IsSerializableType);

            return true;
        }

        public static bool IsSerializableField(FieldInfo field)
        {
            if (field.IsDefined<NonSerializedAttribute>())
                return false;

            if (field.IsLiteral)
                return false;

            if (!(field.IsPublic || field.IsDefined<SerializeField>()))
                return false;

            bool serializable = IsSerializableType(field.FieldType);
            return serializable;
        }

        public static bool IsUnityStructType(Type type)
        {
            for (int i = 0; i < UnityStructTypes.Length; i++)
                if (type == UnityStructTypes[i])
                    return true;
            return false;
        }

        public static readonly Type[] UnityStructTypes =
        {
            typeof(Vector3),
            typeof(Vector2),
            typeof(Vector4),
            typeof(Rect),
            typeof(Quaternion),
            typeof(Matrix4x4),
            typeof(Color),
            typeof(Color32),
            typeof(LayerMask),
            typeof(Bounds)
        };
    }
}
