using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Serialization
{
    public abstract class ISerializationLogic
    {
        public abstract bool IsSerializableField(FieldInfo field);
        public abstract bool IsSerializableProperty(PropertyInfo property);
        public abstract bool IsSerializableType(Type type);

        public bool IsSerializableMember(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Method)
                return false;

            var field = member as FieldInfo;
            if (field != null)
                return IsSerializableField(field);

            var property = member as PropertyInfo;
            if (property != null)
                return IsSerializableProperty(property);

            return false;
        }
    }

    public class VFWSerializationLogic : ISerializationLogic
    {
        private readonly SerializationAttributes attributes;

        public readonly Func<Type, List<RuntimeMember>> GetCachedSerializableMembers;

        public static readonly VFWSerializationLogic Instance = new VFWSerializationLogic(SerializationAttributes.Default);

        public SerializationAttributes Attributes { get { return attributes; } }

        public VFWSerializationLogic(SerializationAttributes attributes)
        {
            this.attributes = attributes;

            GetCachedSerializableMembers = new Func<Type, List<RuntimeMember>>(type =>
                GetSerializableMembers(type, null).ToList()).Memoize();
        }

        public List<RuntimeMember> GetSerializableMembers(Type type, object target)
        {
            var members = ReflectionHelper.CachedGetMembers(type);
            var serializableMembers = members.Where(IsSerializableMember);
            var result = RuntimeMember.WrapMembers(serializableMembers, target);
            return result;
        }

        public bool IsSerializableMember(RuntimeMember member)
        {
            var field = member.Info as FieldInfo;
            return field != null ? IsSerializableField(field) : IsSerializableProperty((PropertyInfo)member.Info);
        }

        public override bool IsSerializableType(Type type)
        {
            if (IsSimpleType(type)
                || type.IsA<UnityObject>()
                || UnityStructs.ContainsValue(type))
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

            return attributes.SerializableType.IsNullOrEmpty() || attributes.SerializableType.Any(type.IsDefined);
        }

        public override bool IsSerializableField(FieldInfo field)
        {
            if (attributes.DontSerializeMember.Any(field.IsDefined))
                return false;

            if (field.IsLiteral)
                return false;

            if (!(field.IsPublic || attributes.SerializeMember.Any(field.IsDefined)))
                return false;

            bool serializable = IsSerializableType(field.FieldType);
            return serializable;
        }

        public override bool IsSerializableProperty(PropertyInfo property)
        {
            if (attributes.DontSerializeMember.Any(property.IsDefined))
                return false;

            if (!property.IsAutoProperty())
                return false;

            if (!(property.GetGetMethod(true).IsPublic ||
                  property.GetSetMethod(true).IsPublic ||
                  attributes.SerializeMember.Any(property.IsDefined)))
                return false;

            bool serializable = IsSerializableType(property.PropertyType);
            return serializable;
        }

        public static readonly Type[] UnityStructs =
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

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive || type.IsEnum || type == typeof(string);
        }
    }
}