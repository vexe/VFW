using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace BX20Serializer
{
    public static class X20Logic
    {
        public static Type[] SerializeMemberAttributes;
        public static Type[] DontSerializeMemberAttributes;

        public static FieldPredicate IsSerializableFieldPredicate;
        public static PropertyPredicate IsSerializablePropertyPredicate;

        public static readonly Func<Type, MemberInfo[]> CachedGetSerializableMembers;

        public static bool IsSerializableMember(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Method)
                return false;

            var field = member as FieldInfo;
            if (field != null)
            {
                if (IsSerializableFieldPredicate != null)
                    return IsSerializableFieldPredicate(field);

                return DefaultIsSerializableField(field);
            }

            var property = member as PropertyInfo;
            if (property != null)
            {
                if (IsSerializablePropertyPredicate != null)
                    return IsSerializablePropertyPredicate(property);

                return DefaultIsSerializableProperty(property);
            }

            return false;
        }

        public static Type[] GetSerializableMembersTypes(Type forType)
        {
            var members = CachedGetSerializableMembers(forType);
            var result = X20Reflection.GetMembersTypes(members);
            return result;
        }


        static X20Logic()
        {
            SerializeMemberAttributes = new Type[] { typeof(SerializeField) };
            DontSerializeMemberAttributes = new Type[] { typeof(NonSerializedAttribute) };

            CachedGetSerializableMembers = new Func<Type, MemberInfo[]>(type =>
            {
                var members = X20Reflection.CachedGetMembers(type);
                var serializableMembers = members.Where(IsSerializableMember).ToArray();
                return serializableMembers;
            }).Memoize();
        }


        public static bool IsSerializableType(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || type == typeof(string)
                || typeof(UnityObject).IsAssignableFrom(type)
                || IsUnityType(type))
                return true;

            if (type.IsArray)
                return type.GetArrayRank() == 1 && IsSerializableType(type.GetElementType());

            if (type.IsInterface)
                return true;

            if (NotSupportedTypes.Any(type.IsA))
                return false;

            if (SupportedTypes.Any(type.IsA))
                return true;

            if (type.IsGenericType)
                return type.GetGenericArguments().All(IsSerializableType);

            return true;
        }

        public static bool DefaultIsSerializableField(FieldInfo field)
        {
            if (DontSerializeMemberAttributes.Any(field.IsDefined))
                return false;

            if (field.IsLiteral)
                return false;

            if (!(field.IsPublic || SerializeMemberAttributes.Any(field.IsDefined)))
                return false;

            bool serializable = IsSerializableType(field.FieldType);
            return serializable;
        }

        public static bool DefaultIsSerializableProperty(PropertyInfo property)
        {
            if (DontSerializeMemberAttributes.Any(property.IsDefined))
                return false;

            if (!property.IsAutoProperty())
                return false;

            if (!(property.GetGetMethod(true).IsPublic ||
                  property.GetSetMethod(true).IsPublic ||
                  SerializeMemberAttributes.Any(property.IsDefined)))
                return false;

            bool serializable = IsSerializableType(property.PropertyType);
            return serializable;
        }

        public static bool IsUnityType(Type type)
        {
            for (int i = 0; i < UnityTypes.Length; i++)
                if (type == UnityTypes[i])
                    return true;
            return false;
        }

        public static readonly Type[] UnityTypes =
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

        public static readonly Type[] NotSupportedTypes =
        {
            typeof(Delegate)
        };

        public static readonly Type[] SupportedTypes =
        {
            typeof(Type)
        };
    }
}
